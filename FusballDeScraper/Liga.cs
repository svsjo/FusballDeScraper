namespace FusballDeScraper;

public class Liga
{
    public List<VereinPlatzierung> Tabelle { get; set; }
    public List<TorschuetzenEintrag> Torschuetzen { get; set; }
    public List<Mannschaft> Mannschaften { get; set; }

    public Liga(string url)
    {
        Tabelle = new List<VereinPlatzierung>();
        Torschuetzen = new List<TorschuetzenEintrag>();
        Mannschaften = new List<Mannschaft>();
    }
}