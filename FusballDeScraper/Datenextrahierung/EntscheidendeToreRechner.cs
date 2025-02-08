using FusballDeScraper.Datenklassen.Mannschaftsdaten;
using FusballDeScraper.Datenklassen.Spiele;
using FusballDeScraper.Datenklassen.Spielereignisse;
using FusballDeScraper.Datenklassen.Tabelle;

namespace FusballDeScraper.Datenextrahierung;

public class TorartSpieler
{
    public int Fuehrungstreffer { get; set; }
    public int Entscheidungstreffer { get; set; }
    public int SpielentscheidendeTore { get; set; }
}

public static class EntscheidendeToreRechner
{
    public static Dictionary<Spieler, TorartSpieler> GetTorartenJeSpieler(List<AbgeschlossenesSpiel> spiele, string mannschaftName)
    {
        var ergebnisse = new Dictionary<Spieler, TorartSpieler>();

        foreach (var spiel in spiele)
        {
            var heimScore = 0;
            var auswaertsScore = 0;

            if (spiel.Spielereignisse == default) continue;

            var listFuehrungstreffer = new List<Tor>();

            var teamArt = spiel.GetTeamEnum(mannschaftName);

            foreach (var tor in spiel.Spielereignisse.OfType<Tor>().OrderBy(t => t.Minute))
            {
                if (tor.Torschuetze == null) continue;

                // Spielstände aktualisieren
                if (tor.Team == Team.HEIM)
                {
                    heimScore++;
                }
                else if (tor.Team == Team.AUSWAERTS)
                {
                    auswaertsScore++;
                }

                // Abbruch wenn anderes Team
                if (teamArt != tor.Team) continue;

                // Init
                if (!ergebnisse.ContainsKey(tor.Torschuetze))
                {
                    ergebnisse[tor.Torschuetze] = new TorartSpieler();
                }

                // Führungstreffer prüfen
                if (IsFuehrungstreffer(heimScore, auswaertsScore, tor.Team))
                {
                    ergebnisse[tor.Torschuetze].Fuehrungstreffer++;
                    listFuehrungstreffer.Add(tor); // zur spaeteren Bestimmung ob Spielentscheidend
                }

                // Entscheidungstreffer prüfen
                if (IsWichtigerTreffer(
                    heimScore,
                    auswaertsScore,
                    spiel.ToreHeim,
                    spiel.ToreAuswaerts,
                    tor.Team))
                {
                    ergebnisse[tor.Torschuetze].Entscheidungstreffer++;
                }
            }

            CheckSpielEntscheidendeTore(heimScore, auswaertsScore, listFuehrungstreffer, ergebnisse, teamArt);
        }

        return ergebnisse;
    }

    private static void CheckSpielEntscheidendeTore(int heimScore, int auswaertsScore, List<Tor> listFuehrungstreffer, Dictionary<Spieler, TorartSpieler> ergebnisse, Team teamArt)
    {
        if (!listFuehrungstreffer.Any()) return;

        if (teamArt == Team.HEIM && heimScore < auswaertsScore || teamArt == Team.AUSWAERTS && auswaertsScore < heimScore) return;

        var relevantGoal = listFuehrungstreffer.Where(x => x.Team == teamArt).OrderByDescending(y => y.Minute).First();

        if (relevantGoal.Torschuetze == default) return;

        ergebnisse[relevantGoal.Torschuetze].SpielentscheidendeTore++;
    }

    // Prüfung: Führungstreffer
    private static bool IsFuehrungstreffer(int aktuelleToreHeim, int aktuelleToreAuswaerts, Team team)
    {
        return (team == Team.HEIM && aktuelleToreHeim == aktuelleToreAuswaerts + 1) ||
               (team == Team.AUSWAERTS && aktuelleToreAuswaerts == aktuelleToreHeim + 1);
    }

    // Prüfung: Entscheidungstreffer
    private static bool IsWichtigerTreffer(
        int aktuelleToreEigenesTeam,
        int aktuelleToreGegner,
        int endstandEigenesTeam,
        int endstandGegner,
        Team aktuellesTeam)
    {
        // Tore gelten nur als entscheidend, wenn das Ergebnis ein Sieg oder Unentschieden ist
        if (endstandEigenesTeam <= endstandGegner)
        {
            return false; // Keine Punkte geholt, keine entscheidenden Tore
        }

        // Ein Tor ist entscheidend, wenn es den Unterschied für den Sieg ausmacht
        return aktuelleToreEigenesTeam <= endstandGegner + 1;
    }
}



