using FusballDeScraper.Datenklassen;
using FusballDeScraper.Datenklassen.Mannschaftsdaten;
using HtmlAgilityPack;

namespace FusballDeScraper.Datenextrahierung;

static class SpielerExtractor
{
    public static async Task<Spieler> GetSpieler(string url, Liga liga)
    {
        if (liga.AlleSpieler.TryGetValue(url, out var vorhandenerSpieler))
        {
            return vorhandenerSpieler;
        }

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

        liga.AlleSpieler.TryAdd(url, spieler);

        return spieler;
    }
}