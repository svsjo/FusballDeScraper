using FusballDeScraper.Datenklassen.Mannschaftsdaten;
using FusballDeScraper.Datenklassen.Spielereignisse;
using FusballDeScraper.Datenklassen.Tabelle;

namespace FusballDeScraper.Datenklassen.Spiele;

public class AbgeschlossenesSpiel : Spiel
{
    public List<Spielereignis>? Spielereignisse { get; set; }
    public List<AufstellungsEintrag>? AufstellungHeim { get; set; }
    public List<AufstellungsEintrag>? AufstellungAuswaerts { get; set; }
    public int ToreHeim { get; set; }
    public int ToreAuswaerts { get; set; }
    public bool Abgesagt { get; set; } = false;

    public Team GetTeamEnum(string mannschaft)
    {
        if (mannschaft == HeimTeam)
        {
            return Team.HEIM;
        }

        if (mannschaft == AuswaertsTeam)
        {
            return Team.AUSWAERTS;
        }

        return Team.BEIDE;
    }

    public Team GetGegnerTeamEnum(string mannschaft)
    {
        if (mannschaft == HeimTeam)
        {
            return Team.AUSWAERTS;
        }

        if (mannschaft == AuswaertsTeam)
        {
            return Team.HEIM;
        }

        return Team.BEIDE;
    }

    public List<Spielereignis>? GetSpielereignisse(string mannschaft)
    {
        if (mannschaft == HeimTeam)
        {
            return Spielereignisse?.Where(x => x.Team == Tabelle.Team.HEIM).ToList();
        }

        if (mannschaft == AuswaertsTeam)
        {
            return Spielereignisse?.Where(x => x.Team == Tabelle.Team.AUSWAERTS).ToList();
        }

        return new();
    }

    public bool HasPlayer(Spieler spieler)
    {
        return AufstellungAuswaerts.Any(x => x.Spieler == spieler) || AufstellungHeim.Any(x => x.Spieler == spieler);
    }

    public bool HasWon(string mannschaft)
    {
        if (mannschaft == HeimTeam)
        {
            return ToreHeim > ToreAuswaerts;
        }

        if (mannschaft == AuswaertsTeam)
        {
            return ToreAuswaerts > ToreHeim;
        }

        return false;
    }

    public bool HasDrawn()
    {
        return ToreHeim == ToreAuswaerts;
    }

    public List<AufstellungsEintrag>? GetAufstellung(string mannschaft)
    {
        if (mannschaft == HeimTeam)
        {
            return AufstellungHeim;
        }

        if (mannschaft == AuswaertsTeam)
        {
            return AufstellungAuswaerts;
        }

        return new();
    }


    public string GetKey()
    {
        return $"{HeimTeam} vs {AuswaertsTeam} - {DateTime}";
    }
}