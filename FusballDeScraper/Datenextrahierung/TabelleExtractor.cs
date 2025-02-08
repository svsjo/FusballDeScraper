using FusballDeScraper.Datenklassen.Tabelle;
using HtmlAgilityPack;

namespace FusballDeScraper.Datenextrahierung;

public static class TabelleExtractor
{
    public static async Task<List<VereinPlatzierung>> GetTabelle(string url)
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

        foreach (HtmlNode row in allRowsRaw!)
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

        Console.WriteLine("===================================================================");
        Console.WriteLine("Tabelle:");
        tabelle.ForEach(x =>
        {
            Console.WriteLine($"{x.Rang}: {x.Mannschaft} | {x.Gewonnen}/{x.Unentschieden}/{x.Verloren} | {x.Tore}:{x.Gegentore} | {x.Tordifferenz} | {x.Punkte}P");
        });
        Console.WriteLine("===================================================================");


        for (var platz = 0; platz < tabelle.Count - 1; platz++)
        {
            tabelle[platz].Vorsprung = tabelle[platz].Punkte - tabelle[platz + 1].Punkte;
        }

        for (var platz = 1; platz < tabelle.Count; platz++)
        {
            tabelle[platz].Rueckstand = tabelle[platz - 1].Punkte - tabelle[platz].Punkte;
        }

        return tabelle;
    }
}
