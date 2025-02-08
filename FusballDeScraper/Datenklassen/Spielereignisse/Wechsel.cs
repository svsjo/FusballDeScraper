using FusballDeScraper.Datenklassen.Mannschaftsdaten;

namespace FusballDeScraper.Datenklassen.Spielereignisse;

public class Wechsel : Spielereignis
{
    public Spieler? Auswechslung { get; set; }
    public Spieler? Einwechslung { get; set; }
}