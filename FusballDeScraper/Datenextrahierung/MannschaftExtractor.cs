using FusballDeScraper.Datenklassen;
using FusballDeScraper.Datenklassen.Mannschaftsdaten;
using HtmlAgilityPack;

namespace FusballDeScraper.Datenextrahierung;

public static class MannschaftExtractor
{
    public static async Task<Mannschaft> GetMannschaft(string url, Liga liga, IProgress<string> progress)
    {
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        string name = htmlDocument.DocumentNode.Descendants("h2").FirstOrDefault()?.InnerText;

        progress.Report($"Lade Mannschaften... {name}");

        Console.WriteLine("\n===================================================================");
        Console.WriteLine($"Mannschaft {name}:");

        var einsaetze = KaderExtractor.GetAlleEinsaetze(htmlDocument, liga);

        var abgeschlossen = await SpieleExtractor.GetAbgeschlosseneSpiele(htmlDocument, liga);

        if (!einsaetze.Any())
        {
            einsaetze = EinsatzMinutenRechner.GetAlleEinsaetze(abgeschlossen, name);
        }

        // Nachtragen in Torschuetzen

        foreach ( var e in einsaetze )
        {
            var searchedEintrag = liga.Torschuetzen.FirstOrDefault(x => x.Spieler == e.Spieler);

            if (searchedEintrag == default) continue;

            searchedEintrag.Einsaetze = e.Einsaetze;
            searchedEintrag.EinsatzMinuten = e.EinsatzMinuten;
        }

        var wichtigeTore = EntscheidendeToreRechner.GetTorartenJeSpieler(abgeschlossen, name);

        foreach (var tor in wichtigeTore)
        {
            var searchedEintrag = liga.Torschuetzen.FirstOrDefault(x => x.Spieler == tor.Key);

            if (searchedEintrag == default) continue;

            searchedEintrag.Fuehrungstreffer = tor.Value.Fuehrungstreffer;
            searchedEintrag.WichtigeTreffer = tor.Value.Entscheidungstreffer;
            searchedEintrag.SiegesbringendesTreffer = tor.Value.SpielentscheidendeTore;
        }

        // ====

        var mannschaft = new Mannschaft()
        {
            Name = name,
            AlleEinsaezte = einsaetze,
            LetzteSpiele = abgeschlossen,
            NaechsteSpiele = SpieleExtractor.GetNaechsteSpiele(htmlDocument, liga)
        };
        Console.WriteLine("===================================================================");

        return mannschaft;
    }
}
