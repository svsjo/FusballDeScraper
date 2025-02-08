using FusballDeScraper.Datenklassen.Spiele;

namespace FusballDeScraper.Datenklassen.Mannschaftsdaten;

public class Mannschaft
{
    public Mannschaft()
    {
        AlleEinsaezte = new List<EinsatzEintrag>();
        NaechsteSpiele = new List<Spiel>();
        LetzteSpiele = new List<AbgeschlossenesSpiel>();
    }

    public string? Name { get; init; }
    public List<EinsatzEintrag> AlleEinsaezte { get; set; }
    public List<Spiel> NaechsteSpiele { get; set; }
    public List<AbgeschlossenesSpiel> LetzteSpiele { get; set; }
}