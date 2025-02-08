namespace FusballDeScraper.Datenklassen.Mannschaftsdaten;

public class TorschuetzenEintrag
{
    public Spieler? Spieler { get; set; }
    public string? Mannschaft { get; set; }
    public int Rang { get; set; }
    public int Tore { get; set; }

    // Nachtraeglich
    public int Einsaetze { get; set; }
    public int EinsatzMinuten { get; set; }
    public double MinutenProTor
    {
        get
        {
            if (Tore == 0 || EinsatzMinuten == 0) return 0;
            return EinsatzMinuten / Tore;
        }
    }
    public double ToreProSpiel
    {
        get
        {
            if (Tore == 0 || Einsaetze == 0) return 0;
            return Tore / Einsaetze;
        }
    }
    public int Fuehrungstreffer { get; set; }
    public int WichtigeTreffer { get; set; } // Ausgeschlossen: Man fuehrt sowieso schon. Beispiel: 4:1 dann zahelt das 1:0 und das 2:1, der Rest aber nicht
    public int SiegesbringendesTreffer { get; set; }
}