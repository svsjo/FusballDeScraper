using FusballDeScraper.Datenklassen;
using FusballDeScraper.Datenklassen.Mannschaftsdaten;
using HtmlAgilityPack;

namespace FusballDeScraper.Datenextrahierung;

public static class KaderExtractor
{
    public static List<EinsatzEintrag> GetAlleEinsaetze(HtmlDocument htmlDocument, Liga liga)
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

        if (section!.Descendants("p").Any(x => x.HasClass("headline")))
        {
            Console.WriteLine("Einsatztabelle nicht freigegeben!");
            return alleEinsaetze;
        }

        var allRowsRaw = section
            .Descendants("table")
            .FirstOrDefault()
            ?.Descendants("tbody")
            .FirstOrDefault()
            ?.Descendants("tr")
            .ToList();

        var rang = 1;

        foreach (HtmlNode row in allRowsRaw!)
        {
            var fields = row.Descendants("td").ToList();

            if (!int.TryParse(fields[1].InnerText, out var einsaetze)) einsaetze = 0;
            if (!int.TryParse(fields[2].InnerText, out var einsatzMinuten)) einsatzMinuten = 0;
            if (!int.TryParse(fields[3].InnerText, out var tore)) tore = 0;

            var einsatz = new EinsatzEintrag()
            {
                Spieler = SpielerExtractor.GetSpieler(fields[0].Descendants("a").FirstOrDefault()!.Attributes["href"].Value, liga).Result,
                Einsaetze = einsaetze,
                EinsatzMinuten = einsatzMinuten,
                Tore = tore,
                Rang = rang
            };

            alleEinsaetze.Add(einsatz);

            rang++;
        }

        Console.WriteLine("\nKaderübersicht:");

        alleEinsaetze.ForEach(x =>
        {
            Console.WriteLine($"{x.Rang}: {x.Spieler?.Name} - {x.Einsaetze} Spiele - {x.EinsatzMinuten} Min - {x.Tore} Tore - {x.MinutenProTor} Min/Tor - {x.MinutenProEinsatz} Min/Einsatz");
        });

        return alleEinsaetze;
    }
}
