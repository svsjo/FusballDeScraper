namespace FusballDeScraper;

public class Wechsel : Spielereignis
{
    public Spieler? Auswechslung { get; set; }
    public Spieler? Einwechslung { get; set; }
}