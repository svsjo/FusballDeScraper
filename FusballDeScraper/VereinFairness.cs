namespace FusballDeScraper;

public class VereinFairness
{
    public int Spiele { get; init; }
    public int GelbeKarten { get; init; }
    public int GelbRoteKarten { get; init; }
    public int RoteKarten { get; init; }
    public int UnfairnessPunkte { get { return GelbeKarten + GelbRoteKarten * 3 + RoteKarten * 5; } }
    public double UnfairnessQuote { get { return UnfairnessPunkte / Spiele; } }
}