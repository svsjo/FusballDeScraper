using FusballDeScraper.Datenklassen.Mannschaftsdaten;
using FusballDeScraper.Datenklassen.Spiele;
using FusballDeScraper.Datenklassen.Spielereignisse;
using FusballDeScraper.Datenklassen.Tabelle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusballDeScraper.Datenklassen.Helper;

public static class AuswertungsHelper
{
    public static double GetWinRateSpieler(Mannschaft mannschaft, Spieler spieler)
    {
        var wins = 0;
        var games = 0;
        
        foreach (var game in mannschaft.LetzteSpiele)
        {
            var aufstellung = game.GetAufstellung(mannschaft.Name);

            var searchedPlayer = aufstellung?.FirstOrDefault(x => x.Spieler == spieler);

            if (searchedPlayer == default || !searchedPlayer.StartAufstellung) continue;

            games++;

            if (game.HasWon(mannschaft.Name)) wins++;
        }



        if (wins == 0 || games == 0)
        {
            return 0.0f;
        }

        return wins / games;
    }

    public static int ToreWaehrendAufDemFeld(Mannschaft mannschaft, Spieler spieler)
    {
        var result = 0;

        foreach (var spiel in mannschaft.LetzteSpiele)
        {
            var team = spiel.GetTeamEnum(mannschaft.Name);

            var tore = GetSpielerEreignisseMitSpielerAufFeld(spiel, spieler, team).Where(x => x is Tor).Count();

            result += tore;
        }

        return result;
    }

    public static int GegenToreWaehrendAufDemFeld(Mannschaft mannschaft, Spieler spieler)
    {
        var result = 0;

        foreach (var spiel in mannschaft.LetzteSpiele)
        {
            var team = spiel.GetGegnerTeamEnum(mannschaft.Name);

            var tore = GetSpielerEreignisseMitSpielerAufFeld(spiel, spieler, team).Where(x => x is Tor).Count();

            result += tore;
        }

        return result;
    }


    public static List<Spielereignis> GetSpielerEreignisseMitSpielerAufFeld(AbgeschlossenesSpiel spiel, Spieler spieler, Team team)
    {
        var aufstellung = spiel.GetAufstellung(spieler.Verein);

        if (aufstellung == default) return new();

        var einsatzEintrag = aufstellung?.FirstOrDefault(x => x.Spieler == spieler);

        if (einsatzEintrag == default) return new();

        var startMin = einsatzEintrag.StartAufstellung ? 0 : einsatzEintrag.MinuteEingewechselt;
        var stopMin = einsatzEintrag.Ausgewechselt ? einsatzEintrag.MinuteAusgewechselt : 90;

        return GetSpielerEreignisseInZeit(spiel.Spielereignisse, startMin, stopMin, team);
    }

    public static List<Spielereignis> GetSpielerEreignisseInZeit(List<Spielereignis> ereignisse, int startMin, int stopMin, Team team = Team.BEIDE)
    {
        var result = ereignisse.Where(x => x.Minute >= startMin && x.Minute <= stopMin);

        if (team is not Team.BEIDE)
        {
            result = result.Where(x => x.Team == team);
        }

        return result.ToList();
    }
}
