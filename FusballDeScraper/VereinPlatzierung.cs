namespace FusballDeScraper;

public class VereinPlatzierung
{
    public int Rang { get; init; }
    public string? Mannschaft { get; set; }
    public int Spiele { get { return Gewonnen + Unentschieden + Verloren; } }
    public int Gewonnen { get; init; }
    public int Unentschieden { get; init; }
    public int Verloren { get; init; }
    public int Tore { get; init; }
    public int Gegentore { get; init; }
    public int Tordifferenz { get { return Tore - Gegentore; } }
    public int Punkte { get { return Gewonnen * 3 + Unentschieden; } }
    public int GelbeKarten { get; init; }
    public int GelbRoteKarten { get; init; }
    public int RoteKarten { get; init; }
    public int UnfairnessPunkte { get { return GelbeKarten + GelbRoteKarten * 3 + RoteKarten * 5; } }
    public double UnfairnessQuite { get { return UnfairnessPunkte / Spiele; } }
    public int UnfairnessRang { get; init; }
}