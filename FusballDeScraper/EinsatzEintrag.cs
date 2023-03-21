namespace FusballDeScraper;

public class EinsatzEintrag
{
    public EinsatzEintrag(string spieler, int einsaetze, int einsatzMinuten, int tore)
    {
        Spieler = spieler;
        Einsaetze = einsaetze;
        EinsatzMinuten = einsatzMinuten;
        Tore = tore;
    }

    public string Spieler { get; init; }
    public int Einsaetze { get; init; }
    public int EinsatzMinuten { get; init; }
    public int Tore { get; init; }
    public double MinutenProTor { get { return EinsatzMinuten / Tore; } }
    public double MinutenProEinsatz { get { return EinsatzMinuten / Einsaetze; } }
}