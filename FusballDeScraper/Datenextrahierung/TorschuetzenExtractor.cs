using FusballDeScraper.Datenklassen;
using FusballDeScraper.Datenklassen.Mannschaftsdaten;
using HtmlAgilityPack;

namespace FusballDeScraper.Datenextrahierung;

public static class TorschuetzenExtractor
{
    public static async Task<List<TorschuetzenEintrag>> GetTorschuetzen(string url, Liga liga)
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

        foreach (HtmlNode row in allRowsRaw!)
        {
            var fields = row.Descendants("td").ToList();
            var torschuetze = new TorschuetzenEintrag()
            {
                Rang = int.Parse(fields[0].InnerText),
                Spieler = SpielerExtractor.GetSpieler(fields[1].Descendants("a").First().Attributes["href"].Value, liga).Result,
                Mannschaft = fields[2].Descendants("div").FirstOrDefault(div => div.HasClass("club-name"))?.InnerText.Replace("\t", "").Replace("\n", ""),
                Tore = int.Parse(fields[3].InnerText),
            };

            alleTorschuetzen.Add(torschuetze);
        }

        Console.WriteLine("\n===================================================================");
        Console.WriteLine("Torjäger:");
        alleTorschuetzen.ForEach(x =>
        {
            Console.WriteLine($"{x.Rang}: {x.Spieler?.Name} - {x.Mannschaft} - {x.Tore} Tore");
        });
        Console.WriteLine("===================================================================");

        return alleTorschuetzen;
    }
}
