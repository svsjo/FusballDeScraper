namespace FusballDeScraper;

public class Karte : Spielereignis
{
    public Kartenart Kartenart { get; set; }

    public Karte(int minute, Kartenart kartenart) : base(minute)
    {
        this.Kartenart = kartenart;
    }
}