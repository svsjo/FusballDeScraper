namespace FusballDeScraper;

public class AbgeschlossenesSpiel : Spiel
{
    public List<Spielereignis>? Spielereignisse { get; set; }
    public List<Spieler>? AufstellungHeim { get; set; }
    public List<Spieler>? AufstellungAuswaerts { get; set; }
    public int ToreHeim { get; set; }
    public int ToreAuswaerts { get; set; }
    public bool Abgesagt { get; set; } = false;
}