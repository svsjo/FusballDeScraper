using FusballDeScraper.Datenklassen.Tabelle;
using FusballDeScraper.Datenklassen.Mannschaftsdaten;
using FusballDeScraper.Datenklassen.Spiele;
using System;
using FusballDeScraper.Datenextrahierung;

namespace FusballDeScraper.Datenklassen;

public class Liga
{
    public List<VereinPlatzierung> Tabelle { get; set; } // TODO JW: Heim Auswärts
    public List<PunkteMatrix> TabellenMatrix { get; set; }
    public List<TorschuetzenEintrag> Torschuetzen { get; set; }
    public List<Mannschaft> Mannschaften { get; set; }
    public Dictionary<string, AbgeschlossenesSpiel> AbgeschlosseneSpiele { get; set; }
    public Dictionary<string, Spieler> AlleSpieler { get; set; }
    public Dictionary<string, Spiel> OffeneSpiele { get; set; }
    public int AktuellerSpieltag { get; set; }
    public int LetzterSpieltag { get; set; }
    public string Name { get; set; }

    public Liga()
    {
        Tabelle = new List<VereinPlatzierung>();
        Torschuetzen = new List<TorschuetzenEintrag>();
        Mannschaften = new List<Mannschaft>();
        AbgeschlosseneSpiele = new Dictionary<string, AbgeschlossenesSpiel>();
        AlleSpieler = new Dictionary<string, Spieler>();
        OffeneSpiele = new Dictionary<string, Spiel>();
        TabellenMatrix = new List<PunkteMatrix>();
        Name = "Unbekannt";
    }

    public void UpdateAktuellerSpieltag()
    {
        var hightestSpieltag = AbgeschlosseneSpiele.Max(x => x.Value.Spieltag);

        LetzterSpieltag = hightestSpieltag;

        AktuellerSpieltag = LetzterSpieltag + 1;
    }

    public List<AbgeschlossenesSpiel> GetAbgeschlosseneSpieleVonSpieltag(int spieltag)
    {
        var result = AbgeschlosseneSpiele.Where(x => x.Value.Spieltag == spieltag).Select(y => y.Value).ToList();

        return result;
    }

    public List<Spiel> GetOffeneSpieleVonSpieltag(int spieltag)
    {
        var result = OffeneSpiele.Where(x => x.Value.Spieltag == spieltag).Select(y => y.Value).ToList();

        return result;
    }
}