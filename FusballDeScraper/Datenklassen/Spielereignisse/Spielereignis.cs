using FusballDeScraper.Datenklassen.Tabelle;

namespace FusballDeScraper.Datenklassen.Spielereignisse;

public abstract class Spielereignis
{
    public int Minute { get; set; }
    public Team Team { get; set; }
}