using FusballDeScraper.Datenklassen.Mannschaftsdaten;

namespace FusballDeScraper.Datenklassen.Spiele;

public class AufstellungsEintrag
{
    public Spieler? Spieler { get; set; }
    public bool StartAufstellung { get; set; }
    public int MinuteEingewechselt { get; set; }
    public int Minuten { get; set; }
    public bool Ausgewechselt { get; set; }
    public int MinuteAusgewechselt { get; set; }
    public int Tore { get; set; }
    public int GelbeKarten { get; set; }
    public int RoteKarten { get; set; }
}