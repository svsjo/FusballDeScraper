namespace FusballDeScraper;

public class VereinPlatzierung
{
    public int Rang { get; init; }
    public string? Mannschaft { get; set; }
    public string? UrlMannschaft { get; set; }
    public int Spiele { get { return Gewonnen + Unentschieden + Verloren; } }
    public int Gewonnen { get; init; }
    public int Unentschieden { get; init; }
    public int Verloren { get; init; }
    public int Tore { get; init; }
    public int Gegentore { get; init; }
    public int Tordifferenz { get { return Tore - Gegentore; } }
    public int Punkte { get { return Gewonnen * 3 + Unentschieden; } }
}