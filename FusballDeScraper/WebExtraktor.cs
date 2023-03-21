using HtmlAgilityPack;
using System.Net.Http;
using System;

namespace FusballDeScraper;

public class WebExtraktor
{
    public async Task<Liga> GetLiga(string url)
    {
        var liga = new Liga();

        liga.Tabelle = await GetTabelle(url);

        var urlScorer = url
            .Replace("spieltagsuebersicht", "torjaeger") + "section/top-scorer";
        liga.Torschuetzen = await GetTorschuetzen(urlScorer);

        var mannschaftsUrls = liga.Tabelle
            .Select(x => x.UrlMannschaft)
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();
        foreach (var mannschaftsUrl in mannschaftsUrls)
        {
            liga.Mannschaften.Add(GetMannschaft(mannschaftsUrl!).Result);
        }

        return liga;
    }

    private async Task<List<VereinPlatzierung>> GetTabelle(string url)
    {
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var tabelle = new List<VereinPlatzierung>();

        var allRowsRaw = htmlDocument
            .DocumentNode
            .Descendants("div")
            .FirstOrDefault(div => div.Id == "fixture-league-tables")
            ?.Descendants("tbody")
            .FirstOrDefault()
            ?.Descendants("tr")
            .ToList();

        foreach (HtmlNode row in allRowsRaw)
        {
            var fields = row.Descendants("td").ToList();
            var vereinPlatzierung = new VereinPlatzierung()
            {
                Rang = int.Parse(fields[1].InnerText.Replace(".", "")),
                Mannschaft = fields[2].Descendants("div").FirstOrDefault(div => div.HasClass("club-name"))?.InnerText.Replace("\t", "").Replace("\n", ""),
                UrlMannschaft = fields[2].Descendants("a").FirstOrDefault()?.Attributes["href"].Value,
                Gewonnen = int.Parse(fields[4].InnerText),
                Unentschieden = int.Parse(fields[5].InnerText),
                Verloren = int.Parse(fields[6].InnerText),
                Tore = int.Parse(fields[7].InnerText.Split(" : ")[0]),
                Gegentore = int.Parse(fields[7].InnerText.Split(" : ")[1]),
            };

            tabelle.Add(vereinPlatzierung);
        }

        return tabelle;
    }

    private async Task<List<TorschuetzenEintrag>> GetTorschuetzen(string url)
    {
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var alleTorschuetzen = new List<TorschuetzenEintrag>();

        var allRowsRaw = htmlDocument
            .DocumentNode
            .Descendants("table")
            .FirstOrDefault()
            ?.Descendants("tbody")
            .FirstOrDefault()
            ?.Descendants("tr")
            .ToList();

        foreach (HtmlNode row in allRowsRaw)
        {
            var fields = row.Descendants("td").ToList();
            var torschuetze = new TorschuetzenEintrag()
            {
                Rang = int.Parse(fields[0].InnerText),
                Spieler = GetSpieler(fields[1].Descendants("a").First().Attributes["href"].Value).Result,
                Mannschaft = fields[2].Descendants("div").FirstOrDefault(div => div.HasClass("club-name"))?.InnerText.Replace("\t", "").Replace("\n", ""),
                Tore = int.Parse(fields[3].InnerText),
            };

            alleTorschuetzen.Add(torschuetze);
        }

        return alleTorschuetzen;
    }

    private async Task<Mannschaft> GetMannschaft(string url)
    {
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var mannschaft = new Mannschaft()
        {
            Name = htmlDocument.DocumentNode.Descendants("h2").FirstOrDefault()?.InnerText,
            AlleEinsaezte = GetAlleEinsaetze(htmlDocument),
            LetzteSpiele = GetAbgeschlosseneSpiele(url),
            NaechsteSpiele = GetNaechsteSpiele(url)
        };

        return mannschaft;
    }

    private List<Spiel> GetNaechsteSpiele(string url)
    {
        var naechsteSpiele = new List<Spiel>();

        return naechsteSpiele;
    }

    private List<AbgeschlossenesSpiel> GetAbgeschlosseneSpiele(string url)
    {
        var letzteSpiele = new List<AbgeschlossenesSpiel>();

        return letzteSpiele;
    }

    private List<EinsatzEintrag> GetAlleEinsaetze(HtmlDocument htmlDocument)
    {
        var alleEinsaetze = new List<EinsatzEintrag>();

        var section = htmlDocument
            .DocumentNode
            .Descendants("section")
            .FirstOrDefault(x => x.Id == "team-squad");

        if (section == default) return alleEinsaetze;

        section = section
            .Descendants("div")
            .FirstOrDefault(x => x.HasClass("team-squad-table"));

        if (section!.Descendants("p").Any(x => x.HasClass("headline"))) return alleEinsaetze;

        var allRowsRaw = section
            .Descendants("table")
            .FirstOrDefault()
            ?.Descendants("tbody")
            .FirstOrDefault()
            ?.Descendants("tr")
            .ToList();

        foreach (HtmlNode row in allRowsRaw)
        {
            var fields = row.Descendants("td").ToList();

            if (!int.TryParse(fields[1].InnerText, out var einsaetze)) einsaetze = 0;
            if (!int.TryParse(fields[2].InnerText, out var einsatzMinuten)) einsatzMinuten = 0;
            if (!int.TryParse(fields[3].InnerText, out var tore)) tore = 0;

            var einsatz = new EinsatzEintrag()
            {
                Spieler = GetSpieler(fields[0].Descendants("a").FirstOrDefault()!.Attributes["href"].Value).Result,
                Einsaetze = einsaetze,
                EinsatzMinuten = einsatzMinuten,
                Tore = tore
            };

            alleEinsaetze.Add(einsatz);
        }

        return alleEinsaetze;
    }

    private async Task<Spieler> GetSpieler(string url)
    {
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var datas = htmlDocument
            .DocumentNode
            .Descendants("div")
            .FirstOrDefault(x => x.HasClass("stage-profile"))
            ?.Descendants("p")
            .ToList();

        var name = datas
            .FirstOrDefault(x => x.HasClass("profile-name"))
            ?.InnerText;

        var verein = datas
            .FirstOrDefault(x => x.HasClass("profile-player-team"))
            ?.Descendants("a")
            .FirstOrDefault()
            ?.InnerText;

        var spieler = new Spieler()
        {
            Name = name ?? "Unbekannt",
            Verein = verein ?? "Unbekannt",
            UrlSpieler = url
        };

        return spieler;
    }

    private Spiel GetSpiel(string url)
    {
        var spiel = new Spiel();

        return spiel;
    }

    private AbgeschlossenesSpiel GetAbgeschlossenesSpiel(string url)
    {
        var abgeschlossenesSpiel = new AbgeschlossenesSpiel();

        return abgeschlossenesSpiel;
    }
}