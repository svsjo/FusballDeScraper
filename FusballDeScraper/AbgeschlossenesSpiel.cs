namespace FusballDeScraper;

public class AbgeschlossenesSpiel : Spiel
{
    public List<Spielereignis>? Spielereignisse { get; set; }
    public int ToreHeim { get; set; }
    public int ToreAuswaerts { get; set; }
}