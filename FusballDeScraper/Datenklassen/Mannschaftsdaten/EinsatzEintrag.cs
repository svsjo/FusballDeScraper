namespace FusballDeScraper.Datenklassen.Mannschaftsdaten;

public class EinsatzEintrag
{
    public Spieler? Spieler { get; init; }
    public int Rang { get; set; }
    public int Einsaetze { get; set; }
    public int EinsatzMinuten { get; set; }
    public int Tore { get; set; }
    public double MinutenProTor 
    { 
        get 
        {
            if (Tore == 0 || EinsatzMinuten == 0) return 0;
            return EinsatzMinuten / Tore; 
        } 
    }
    public double MinutenProEinsatz 
    { 
        get 
        {
            if (Einsaetze == 0 || EinsatzMinuten == 0) return 0;
            return EinsatzMinuten / Einsaetze; 
        } 
    }
    
    // Karten nicht zuordnenbar, daher hier nicht enthalten
}