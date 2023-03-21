namespace FusballDeScraper;

public abstract class Spielereignis
{
    public int Minute { get; set; }

    protected Spielereignis(int minute)
    {
        Minute = minute;
    }
}