using FusballDeScraper.Datenklassen;
using FusballDeScraper.Datenklassen.Spielereignisse;
using FusballDeScraper.Datenklassen.Tabelle;
using HtmlAgilityPack;

namespace FusballDeScraper.Datenextrahierung;

public static class SpielereignisExtractor
{
    public static List<Spielereignis> GetSpielEreignisse(List<HtmlNode> events, Liga liga)
    {
        var spielereignisse = new List<Spielereignis>();

        foreach (var spielEvent in events)
        {
            var minuteString = spielEvent
                .Descendants("div")
                .First(x => x.HasClass("column-time"))
                .Descendants("div")
                .First(x => x.HasClass("valign-inner"))
                .InnerText
                .Replace("&rsquo;", "");

            var minute = GetHandledNachspielZeit(minuteString);

            var team = spielEvent.HasClass("event-right") ? Team.AUSWAERTS : Team.HEIM;

            var playerOrEvent = spielEvent
                .Descendants("div")
                .First(x => x.HasClass("column-player"))
                .Descendants("div")
                .First(x => x.HasClass("valign-cell"));

            var player = playerOrEvent.Descendants("a").FirstOrDefault();
            var spielerWechsel = playerOrEvent.Descendants("div").Where(x => x.HasClass("substitute")).ToList();

            if (player == default && !spielerWechsel.Any())
            {
                /* Karte */
                var karte = playerOrEvent.InnerText;

                var kartenArt = karte switch
                {
                    "Gelbe Karte" => Kartenart.GELB,
                    "Gelb-Rote Karte" => Kartenart.GELBROT,
                    "Rote Karte" => Kartenart.ROT,
                    _ => Kartenart.UNDEFINIERT
                };

                var karteEreignis = new Karte()
                {
                    Minute = minute,
                    Team = team,
                    Kartenart = kartenArt
                };

                spielereignisse.Add(karteEreignis);
            }
            else if (!spielerWechsel.Any() && player != default)
            {
                /* Tor */
                var tor = new Tor()
                {
                    Minute = minute,
                    Team = team,
                    Torschuetze = SpielerExtractor.GetSpieler(player.Attributes["href"].Value, liga).Result
                };

                spielereignisse.Add(tor);
            }
            else /* Auswechslung */
            {
                if (spielerWechsel.Count < 2) continue;

                if (!spielerWechsel.ElementAt(0).Descendants("a").Any() || !spielerWechsel.ElementAt(1).Descendants("a").Any()) continue;

                var wechsel = new Wechsel()
                {
                    Team = team,
                    Minute = minute,
                    Einwechslung = SpielerExtractor.GetSpieler(spielerWechsel.ElementAt(0).Descendants("a").FirstOrDefault()!.Attributes["href"].Value, liga).Result,
                    Auswechslung = SpielerExtractor.GetSpieler(spielerWechsel.ElementAt(1).Descendants("a").FirstOrDefault()!.Attributes["href"].Value, liga).Result
                };

                spielereignisse.Add(wechsel);
            }
        }

        return spielereignisse;
    }

    private static int GetHandledNachspielZeit(string minuteString)
    {
        if (!minuteString.Contains("+")) return int.Parse(minuteString);
        var minutes = minuteString.Split("+");
        if (minutes.Length > 1) return int.Parse(minutes[0]) + int.Parse(minutes[1]);
        return int.Parse(minutes[0]);
    }
}