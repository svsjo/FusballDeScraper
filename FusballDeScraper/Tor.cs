namespace FusballDeScraper;

public class Tor : Spielereignis
{
    public Tor(int minute) : base(minute)
    {
    }

    public string? Torschuetze { get; set; }
}