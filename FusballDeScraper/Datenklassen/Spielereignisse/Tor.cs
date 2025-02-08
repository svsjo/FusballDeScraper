using FusballDeScraper.Datenklassen.Mannschaftsdaten;

namespace FusballDeScraper.Datenklassen.Spielereignisse;

public class Tor : Spielereignis
{
    public Spieler? Torschuetze { get; set; }
}