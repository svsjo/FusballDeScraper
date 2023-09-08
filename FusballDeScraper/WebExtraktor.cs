using HtmlAgilityPack;
using System.Net.Http;
using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

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
            LetzteSpiele = GetAbgeschlosseneSpiele(htmlDocument).Result,
            NaechsteSpiele = GetNaechsteSpiele(htmlDocument)
        };

        return mannschaft;
    }

    private List<Spiel> GetNaechsteSpiele(HtmlDocument htmlDocument)
    {
        var naechsteSpiele = new List<Spiel>();

        var allRowsRaw = htmlDocument
            .DocumentNode
            .Descendants("div")
            .FirstOrDefault(x => x.Id == "id-team-matchplan-table")
            ?.Descendants("table")
            .FirstOrDefault()
            ?.Descendants("tbody")
            .FirstOrDefault()
            ?.Descendants("tr")
            .ToList();

        var importantRows = new List<HtmlNode>();

        for (var i = 0; i < allRowsRaw.Count; i++)
        {
            if ((i + 1) % 3 == 0)
            {
                importantRows.Add(allRowsRaw.ElementAt(i));
            }
        }

        foreach (var row in importantRows)
        {
            var url = row.Descendants("td").First(x => x.HasClass("column-detail")).Descendants("a").First().Attributes["href"].Value;

            naechsteSpiele.Add(GetSpiel(url).Result);
        }

        return naechsteSpiele;
    }

    private async Task<List<AbgeschlossenesSpiel>> GetAbgeschlosseneSpiele(HtmlDocument oldHtmlDocument)
    {
        var newUrl = oldHtmlDocument
            .DocumentNode
            .Descendants("section")
            .FirstOrDefault(x => x.Id == "id-team-matchplan")
            ?.Descendants("li")
            .ElementAt(1)
            .Descendants("a")
            .FirstOrDefault()
            ?.Attributes["href"]
            .Value;

        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(newUrl);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var letzteSpiele = new List<AbgeschlossenesSpiel>();

        var allRowsRaw = htmlDocument
            .DocumentNode
            .Descendants("div")
            .FirstOrDefault(x => x.Id == "id-team-matchplan-table")
            ?.Descendants("table")
            .FirstOrDefault()
            ?.Descendants("tbody")
            .FirstOrDefault()
            ?.Descendants("tr")
            .ToList();

        var importantRows = new List<HtmlNode>();

        for (var i = 0; i < allRowsRaw.Count; i++)
        {
            if ((i + 1) % 3 == 0)
            {
                importantRows.Add(allRowsRaw.ElementAt(i));
            }
        }

        foreach (var row in importantRows)
        {
            var url = row.Descendants("td").First(x => x.HasClass("column-detail")).Descendants("a").First().Attributes["href"].Value;

            letzteSpiele.Add(GetAbgeschlossenesSpiel(url).Result);
        }

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

    private async Task<Spiel> GetSpiel(string url)
    {
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var containerInfos = htmlDocument
            .DocumentNode
            ?.Descendants("section")
            .FirstOrDefault(x => x.Id == "stage")
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("stage-content"));

        var ortString = containerInfos
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("stage-header"))
            ?.Descendants("a")
            .FirstOrDefault(x => x.HasClass("location"))
            ?.InnerText
            .Replace("\t", "")
            .Replace("\n", "");

        var splitOrt = new List<string>();
        if (string.IsNullOrEmpty(ortString))
        {
            splitOrt.Add("Unbekannt");
        }
        else
        {
            splitOrt = ortString.Split(",").ToList();
        }

        var platz = splitOrt[0];
        splitOrt.RemoveAt(0);

        var restString = "Unbekannt";
        if (splitOrt.Count > 1)
        {
            restString = splitOrt.Aggregate((x, y) => x + y);
        }


        var mannschaftString = containerInfos
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("stage-body"))
            ?.Descendants("div")
            .ToList();

        var homeTeam = mannschaftString
            ?.FirstOrDefault(x => x.HasClass("team-home"))
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("team-name"))
            ?.Descendants("a")
            .FirstOrDefault()
            ?.InnerText
            .Replace("\t", "")
            .Replace("\n", "");

        var auswaertsTeam = mannschaftString
            ?.FirstOrDefault(x => x.HasClass("team-away"))
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("team-name"))
            ?.Descendants("a")
            .FirstOrDefault()
            ?.InnerText
            .Replace("\t", "")
            .Replace("\n", "");

        var spiel = new Spiel()
        {
            HeimTeam = homeTeam,
            AuswaertsTeam = auswaertsTeam,
            Platzart = platz,
            Ort = restString,
        };

        return spiel;
    }

    private async Task<AbgeschlossenesSpiel> GetAbgeschlossenesSpiel(string url)
    {
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var containerInfos = htmlDocument
            .DocumentNode
            .Descendants("section")
            .FirstOrDefault(x => x.Id == "stage")
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("stage-content"));

        var ortString = containerInfos
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("stage-header"))
            ?.Descendants("a")
            .FirstOrDefault(x => x.HasClass("location"))
            ?.InnerText
            .Replace("\t", "")
            .Replace("\n", "");

        var splitOrt = new List<string>();

        if (string.IsNullOrEmpty(ortString))
        {
            splitOrt.Add("Unbekannt");
        }
        else
        {
            splitOrt = ortString.Split(",").ToList();
        }

        var platz = splitOrt[0];
        splitOrt.RemoveAt(0);

        var restString = "Unbekannt";
        if (splitOrt.Count > 1)
        {
            restString = splitOrt.Aggregate((x, y) => x + y);
        }

        var mannschaftString = containerInfos
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("stage-body"))
            ?.Descendants("div")
            .ToList();

        var homeTeam = mannschaftString
            ?.FirstOrDefault(x => x.HasClass("team-home"))
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("team-name"))
            ?.Descendants("a")
            .FirstOrDefault()
            ?.InnerText
            .Replace("\t", "")
            .Replace("\n", "");

        var auswaertsTeam = mannschaftString
            ?.FirstOrDefault(x => x.HasClass("team-away"))
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("team-name"))
            ?.Descendants("a")
            .FirstOrDefault()
            ?.InnerText
            .Replace("\t", "")
            .Replace("\n", "");

        var spielverlauf = htmlDocument
            .DocumentNode
            ?.Descendants("section")
            .FirstOrDefault(x => x.Id == "course")
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("match-course"))
            ?.Descendants("div")
            .Where(x => x.HasClass("events"))
            .ToList();

        var events = new List<HtmlNode>();

        if (spielverlauf != default)
        {
            foreach (var halbzeit in spielverlauf)
            {
                events.AddRange(halbzeit
                    .Descendants("div")
                    .First(x => x.HasClass("container"))
                    .Descendants("div")
                    .Where(x => x.HasClass("row-event"))
                    .ToList());
            }
        }

        var abgesagt = events.Count == 0 ? true : false;

        var spielereignisse = GetSpielEreignisse(events);

        var toreHeim = spielereignisse.Count(x => x is Tor && x.Team == Team.HEIM);
        var toreAusw = spielereignisse.Count(x => x is Tor && x.Team == Team.AUSWAERTS);

        var spiel = new AbgeschlossenesSpiel()
        {
            HeimTeam = homeTeam,
            AuswaertsTeam = auswaertsTeam,
            Platzart = platz,
            Ort = restString,
            ToreAuswaerts = toreAusw,
            ToreHeim = toreHeim,
            Spielereignisse = spielereignisse,
            Abgesagt = abgesagt
        };

        return spiel;
    }

    private List<Spielereignis> GetSpielEreignisse(List<HtmlNode> events)
    {
        var spielereignisse = new List<Spielereignis>();

        foreach (var spielEvent in events)
        {
            var minuteString = spielEvent
                .Descendants("div")
                .First(x => x.HasClass("column-time"))
                .Descendants("div")
                .First(x => x.HasClass("valign-inner"))
                .InnerText
                .Replace("&rsquo;", "");

            var minute = HandleNachspielZeit(minuteString);

            var team = spielEvent.HasClass("event-right") ? Team.AUSWAERTS : Team.HEIM;

            var playerOrEvent = spielEvent
                .Descendants("div")
                .First(x => x.HasClass("column-player"))
                .Descendants("div")
                .First(x => x.HasClass("valign-cell"));

            var player = playerOrEvent.Descendants("a").FirstOrDefault();
            var spielerWechsel = playerOrEvent.Descendants("div").Where(x => x.HasClass("substitute")).ToList();

            if (player == default && !spielerWechsel.Any())
            {
                /* Karte */
                var karte = playerOrEvent.InnerText;
                var kartenArt = karte switch
                {
                    "Gelbe Karte" => Kartenart.GELB,
                    "Gelb-Rote Karte" => Kartenart.GELBROT,
                    "Rote Karte" => Kartenart.ROT,
                    _ => throw new ArgumentOutOfRangeException()
                };

                var karteEreignis = new Karte()
                {
                    Minute = minute,
                    Team = team,
                    Kartenart = kartenArt
                };

                spielereignisse.Add(karteEreignis);
            }
            else if (!spielerWechsel.Any())
            {
                /* Tor */
                var tor = new Tor()
                {
                    Minute = minute,
                    Team = team,
                    Torschuetze = GetSpieler(player.Attributes["href"].Value).Result
                };

                spielereignisse.Add(tor);
            }
            else /* Auswechslung */
            {
                if(spielerWechsel.Count < 2) continue;

                var wechsel = new Wechsel()
                {
                    Team = team,
                    Minute = minute,
                    Einwechslung = GetSpieler(spielerWechsel.ElementAt(0).Descendants("a").FirstOrDefault()!.Attributes["href"].Value).Result,
                    Auswechslung = GetSpieler(spielerWechsel.ElementAt(1).Descendants("a").FirstOrDefault()!.Attributes["href"].Value).Result
                };

                spielereignisse.Add(wechsel);
            }
        }

        return spielereignisse;
    }

    private int HandleNachspielZeit(string minuteString)
    {
        if (!minuteString.Contains("+")) return int.Parse(minuteString);
        var minutes = minuteString.Split("+");
        return int.Parse(minutes[0]) + int.Parse(minutes[1]);
    }
}