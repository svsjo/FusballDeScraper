using System.Text.RegularExpressions;

namespace FusballDeScraper.Datenklassen.Helper;

public static class SpielklasseExtractor
{
    public static string ExtractSpielklasse(string url)
    {
        // Regex zum Extrahieren des Spielklassen-Teils
        var regex = new Regex(@"spieltagsuebersicht/([^/]+)/");
        var match = regex.Match(url);

        if (!match.Success)
        {
            return "Name nicht extrahiert!";
        }

        // Extrahierter Spielklassen-Teil
        var rawSpielklasse = match.Groups[1].Value;

        // Nachbearbeitung: Doppelwörter entfernen, Bindestriche durch Leerzeichen ersetzen
        var words = rawSpielklasse.Split('-')
            .Distinct() // Doppelte Wörter entfernen
            .Select(w => char.ToUpper(w[0]) + w.Substring(1)) // Großschreibung
            .ToList();

        // Ergebnis zusammenfügen
        return string.Join(" ", words);
    }
}