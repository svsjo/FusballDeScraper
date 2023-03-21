namespace FusballDeScraper;

public class EinsatzEintrag
{
    public Spieler? Spieler { get; init; }
    public int Einsaetze { get; init; }
    public int EinsatzMinuten { get; init; }
    public int Tore { get; init; }
    public double MinutenProTor { get { return EinsatzMinuten / Tore; } }
    public double MinutenProEinsatz { get { return EinsatzMinuten / Einsaetze; } }
}