using FusballDeScraper.Datenklassen.Mannschaftsdaten;
using FusballDeScraper.Datenklassen.Spiele;
using FusballDeScraper.Datenklassen.Spielereignisse;
using System.ComponentModel.DataAnnotations;

namespace FusballDeScraper.Datenextrahierung;

public static class EinsatzMinutenRechner
{
    public static List<EinsatzEintrag> GetAlleEinsaetze(List<AbgeschlossenesSpiel> spiele, string teamName)
    {
        var einsaetzeGesamt = new Dictionary<Spieler, EinsatzEintrag>();

        Console.WriteLine("\n===================================================================");
        Console.WriteLine("Alternativer Kader:");
        foreach (var spiel in spiele)
        {
            if (spiel == default) continue;

            var aufstellungSpiel = new List<AufstellungsEintrag>();

            var aufstellung = spiel.GetAufstellung(teamName);

            if (aufstellung == default) continue;

            foreach (AufstellungsEintrag playerEintrag in aufstellung)
            {
                if (playerEintrag.Spieler == default) continue;

                if (!einsaetzeGesamt.ContainsKey(playerEintrag.Spieler))
                {
                    einsaetzeGesamt.TryAdd(playerEintrag.Spieler, new() { Spieler = playerEintrag.Spieler });
                }

                einsaetzeGesamt[playerEintrag.Spieler].Einsaetze++;
                einsaetzeGesamt[playerEintrag.Spieler].EinsatzMinuten += playerEintrag.Minuten;
                einsaetzeGesamt[playerEintrag.Spieler].Tore += playerEintrag.Tore;
            }
        }

        var result = einsaetzeGesamt.Values.OrderByDescending(x => x.EinsatzMinuten).ToList();

        for (var i = 0; i < result.Count; i++)
        {
            result.ElementAt(i).Rang = i + 1;
        }

        result.ForEach(x => 
        {
            Console.WriteLine($"{x.Rang}: {x.Spieler?.Name} - {x.Einsaetze} Spiele - {x.EinsatzMinuten} Min - {x.Tore} Tore - {x.MinutenProTor} Min/Tor - {x.MinutenProEinsatz} Min/Einsatz");
        });

        Console.WriteLine("===================================================================");

        return result;
    }

    public static List<AufstellungsEintrag> CheckEinsaetzeFuerSpiel(List<Spieler> aufstellung, List<Spielereignis> ereignisse)
    {
        var spielSpielerMinDict = new Dictionary<Spieler, AufstellungsEintrag>();

        foreach (var spieler in aufstellung)
        {
            spielSpielerMinDict.TryAdd(spieler, new() { Minuten = 90 });
        }

        foreach (var ereignis in ereignisse)
        {
            if (ereignis is Wechsel wechsel)
            {
                if (wechsel.Einwechslung != default)
                {
                    spielSpielerMinDict.TryAdd(wechsel.Einwechslung, new() { Minuten = 90 - wechsel.Minute, MinuteEingewechselt = wechsel.Minute });
                }

                if (wechsel.Auswechslung != default)
                {
                    spielSpielerMinDict[wechsel.Auswechslung].Minuten = spielSpielerMinDict[wechsel.Auswechslung].Minuten - (90 - wechsel.Minute); // berücksichtigung ein und auswechslung
                    spielSpielerMinDict[wechsel.Auswechslung].MinuteAusgewechselt = wechsel.Minute;
                    // Bspw. ein 15, aus 80, dann wäre die Rechnung: 75 - (90 - 90) = 65
                }
            }
            else if (ereignis is Tor tor)
            {
                if (tor.Torschuetze != default)
                {
                    spielSpielerMinDict[tor.Torschuetze].Tore++;
                }
            }
        }

        return spielSpielerMinDict.Values.ToList();
    }
}
