using FusballDeScraper.Datenklassen;
using FusballDeScraper.Datenklassen.Mannschaftsdaten;
using HtmlAgilityPack;

namespace FusballDeScraper.Datenextrahierung;

public static class AufstellungExtractor
{
    public static (List<Spieler> heim, List<Spieler> auswaerts) GetStartAufstellung(HtmlDocument htmlDocument, Liga liga)
    {
        List<Spieler> heim = new();
        List<Spieler> auswaerts = new();

        var playerLinks = htmlDocument.DocumentNode.SelectNodes("//a[contains(@href, '/spielerprofil/')]");

        heim = playerLinks?.Select(x => SpielerExtractor.GetSpieler(x.Attributes["href"].Value, liga).Result).ToList() ?? new();
        auswaerts = playerLinks?.Select(x => SpielerExtractor.GetSpieler(x.Attributes["href"].Value, liga).Result).ToList() ?? new();

        return (heim, auswaerts);
    }
}
