using FusballDeScraper.Datenklassen;
using FusballDeScraper.Datenklassen.Spiele;
using FusballDeScraper.Datenklassen.Spielereignisse;
using FusballDeScraper.Datenklassen.Tabelle;
using HtmlAgilityPack;

namespace FusballDeScraper.Datenextrahierung;

public static class SpieleExtractor
{
    public static List<Spiel> GetNaechsteSpiele(HtmlDocument htmlDocument, Liga liga)
    {
        var naechsteSpiele = new List<Spiel>();

        var allRowsRaw = htmlDocument
            .DocumentNode
            .Descendants("div")
            .FirstOrDefault(x => x.Id == "id-team-matchplan-table")
            ?.Descendants("table")
            .FirstOrDefault()
            ?.Descendants("tbody")
            .FirstOrDefault()
            ?.Descendants("tr")
            .ToList();

        var importantRows = new List<HtmlNode>();

        for (var i = 0; i < allRowsRaw?.Count; i++)
        {
            if ((i + 1) % 3 == 0)
            {
                importantRows.Add(allRowsRaw.ElementAt(i));
            }
        }

        Console.WriteLine("\nNächste Spiele:");

        foreach (var row in importantRows)
        {
            var url = row.Descendants("td").First(x => x.HasClass("column-detail")).Descendants("a").First().Attributes["href"].Value;

            naechsteSpiele.Add(GetSpiel(url, liga).Result);
        }

        return naechsteSpiele;
    }

    public static async Task<List<AbgeschlossenesSpiel>> GetAbgeschlosseneSpiele(HtmlDocument oldHtmlDocument, Liga liga)
    {
        var newUrl = oldHtmlDocument
            .DocumentNode
            .Descendants("section")
            .FirstOrDefault(x => x.Id == "id-team-matchplan") // TODO JW: Ggf. bearbeiten
            ?.Descendants("li")
            .ElementAt(1)
            .Descendants("a")
            .FirstOrDefault()
            ?.Attributes["href"]
            .Value;

        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(newUrl);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var letzteSpiele = new List<AbgeschlossenesSpiel>();

        var allRowsRaw = htmlDocument
            .DocumentNode
            .Descendants("div")
            .FirstOrDefault(x => x.Id == "id-team-matchplan-table")
            ?.Descendants("table")
            .FirstOrDefault()
            ?.Descendants("tbody")
            .FirstOrDefault()
            ?.Descendants("tr")
            .ToList();

        var importantRows = new List<HtmlNode>();

        for (var i = 0; i < allRowsRaw!.Count; i++)
        {
            if ((i + 1) % 3 == 0)
            {
                importantRows.Add(allRowsRaw.ElementAt(i));
            }
        }

        Console.WriteLine("\nLetzte Spiele:");

        foreach (var row in importantRows)
        {
            var url = row.Descendants("td").First(x => x.HasClass("column-detail")).Descendants("a").First().Attributes["href"].Value;

            letzteSpiele.Add(await GetAbgeschlossenesSpiel(url, liga));
        }

        return letzteSpiele;
    }

    private static async Task<Spiel> GetSpiel(string url, Liga liga)
    {
        if (liga.OffeneSpiele.TryGetValue(url, out var vorhandenesSpiel))
        {
            return vorhandenesSpiel;
        }

        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var containerInfos = htmlDocument
            .DocumentNode
            ?.Descendants("section")
            .FirstOrDefault(x => x.Id == "stage")
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("stage-content"));

        var ortString = containerInfos
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("stage-header"))
            ?.Descendants("a")
            .FirstOrDefault(x => x.HasClass("location"))
            ?.InnerText
            .Replace("\t", "")
            .Replace("\n", "");

        var splitOrt = new List<string>();
        if (string.IsNullOrEmpty(ortString))
        {
            splitOrt.Add("Unbekannt");
        }
        else
        {
            splitOrt = ortString.Split(",").ToList();
        }

        var platz = splitOrt[0];
        splitOrt.RemoveAt(0);

        var restString = "Unbekannt";
        if (splitOrt.Count > 1)
        {
            restString = splitOrt.Aggregate((x, y) => x + y);
        }


        var mannschaftString = containerInfos
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("stage-body"))
            ?.Descendants("div")
            .ToList();

        var homeTeam = mannschaftString
            ?.FirstOrDefault(x => x.HasClass("team-home"))
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("team-name"))
            ?.Descendants("a")
            .FirstOrDefault()
            ?.InnerText
            .Replace("\t", "")
            .Replace("\n", "");

        var auswaertsTeam = mannschaftString
            ?.FirstOrDefault(x => x.HasClass("team-away"))
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("team-name"))
            ?.Descendants("a")
            .FirstOrDefault()
            ?.InnerText
            .Replace("\t", "")
            .Replace("\n", "");

        var spiel = new Spiel()
        {
            HeimTeam = homeTeam,
            AuswaertsTeam = auswaertsTeam,
            Platzart = platz,
            Ort = restString,
        };

        Console.WriteLine($"Spiel: {spiel.HeimTeam} vs {spiel.AuswaertsTeam} - {spiel.Platzart} {spiel.Ort}");

        liga.OffeneSpiele.TryAdd(url, spiel);

        return spiel;
    }

    private static async Task<AbgeschlossenesSpiel> GetAbgeschlossenesSpiel(string url, Liga liga)
    {
        if (liga.AbgeschlosseneSpiele.TryGetValue(url, out var vorhandenesSpiel))
        {
            return vorhandenesSpiel;
        }

        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var containerInfos = htmlDocument
            .DocumentNode
            .Descendants("section")
            .FirstOrDefault(x => x.Id == "stage")
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("stage-content"));

        var ortString = containerInfos
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("stage-header"))
            ?.Descendants("a")
            .FirstOrDefault(x => x.HasClass("location"))
            ?.InnerText
            .Replace("\t", "")
            .Replace("\n", "");

        var splitOrt = new List<string>();

        if (string.IsNullOrEmpty(ortString))
        {
            splitOrt.Add("Unbekannt");
        }
        else
        {
            splitOrt = ortString.Split(",").ToList();
        }

        var platz = splitOrt[0];
        splitOrt.RemoveAt(0);

        var restString = "Unbekannt";
        if (splitOrt.Count > 1)
        {
            restString = splitOrt.Aggregate((x, y) => x + y);
        }

        var mannschaftString = containerInfos
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("stage-body"))
            ?.Descendants("div")
            .ToList();

        var homeTeam = mannschaftString
            ?.FirstOrDefault(x => x.HasClass("team-home"))
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("team-name"))
            ?.Descendants("a")
            .FirstOrDefault()
            ?.InnerText
            .Replace("\t", "")
            .Replace("\n", "");

        var auswaertsTeam = mannschaftString
            ?.FirstOrDefault(x => x.HasClass("team-away"))
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("team-name"))
            ?.Descendants("a")
            .FirstOrDefault()
            ?.InnerText
            .Replace("\t", "")
            .Replace("\n", "");

        var spielverlauf = htmlDocument
            .DocumentNode
            ?.Descendants("section")
            .FirstOrDefault(x => x.Id == "course")
            ?.Descendants("div")
            .FirstOrDefault(x => x.HasClass("match-course"))
            ?.Descendants("div")
            .Where(x => x.HasClass("events"))
            .ToList();

        var events = new List<HtmlNode>();

        if (spielverlauf != default)
        {
            foreach (var halbzeit in spielverlauf)
            {
                events.AddRange(halbzeit
                    .Descendants("div")
                    .First(x => x.HasClass("container"))
                    .Descendants("div")
                    .Where(x => x.HasClass("row-event"))
                    .ToList());
            }
        }

        var abgesagt = events.Count == 0 ? true : false;

        var spielereignisse = SpielereignisExtractor.GetSpielEreignisse(events, liga);

        var (heimAuf, auswaertsAuf) = AufstellungExtractor.GetStartAufstellung(htmlDocument, liga); // TODO JW: Fix

        var toreHeim = spielereignisse.Count(x => x is Tor && x.Team == Team.HEIM);
        var toreAusw = spielereignisse.Count(x => x is Tor && x.Team == Team.AUSWAERTS);

        var aufstellungHeim = EinsatzMinutenRechner.CheckEinsaetzeFuerSpiel(heimAuf, spielereignisse);
        var aufstellungAusw = EinsatzMinutenRechner.CheckEinsaetzeFuerSpiel(auswaertsAuf, spielereignisse);

        var spiel = new AbgeschlossenesSpiel()
        {
            HeimTeam = homeTeam,
            AuswaertsTeam = auswaertsTeam,
            Platzart = platz,
            Ort = restString,
            ToreAuswaerts = toreAusw,
            ToreHeim = toreHeim,
            Spielereignisse = spielereignisse,
            Abgesagt = abgesagt,
            AufstellungHeim = aufstellungHeim,
            AufstellungAuswaerts = aufstellungAusw
        };

        // TODO JW: Spieltag und Datum, aussedem dann Logik das jedes Spiel mir einmal ausgelesen wird

        Console.WriteLine($"\nSpiel: {spiel.HeimTeam} ({spiel.ToreHeim}) vs ({spiel.ToreAuswaerts}) {spiel.AuswaertsTeam} - {spiel.Platzart} {spiel.Ort} - Abgesagt: {spiel.Abgesagt}");

        var heimString = "\nAustellung heim: ";
        spiel.AufstellungHeim.ForEach(x =>
        {
            heimString += $"{x.Spieler?.Name}, ";
        });
        Console.WriteLine(heimString);

        var auswaertsString = "\nAustellung auswärts: ";
        spiel.AufstellungAuswaerts.ForEach(x =>
        {
            auswaertsString += $"{x.Spieler?.Name}, ";
        });
        Console.WriteLine(auswaertsString);

        Console.WriteLine("\nEreignisse im Spiel:");
        spiel.Spielereignisse.ForEach(x =>
        {
            if (x is Wechsel wechsel)
            {
                Console.WriteLine($"Wechsel {x.Team} - {x.Minute} Minute: {wechsel.Einwechslung?.Name} für {wechsel.Auswechslung?.Name}");
            }
            else if (x is Karte karte)
            {
                Console.WriteLine($"Karte {x.Team} - {x.Minute} Minute: {karte.Kartenart}");
            }
            else if (x is Tor tor)
            {
                Console.WriteLine($"Tor {x.Team} - {x.Minute} Minute: {tor.Torschuetze?.Name}");
            }
        });

        liga.AbgeschlosseneSpiele.TryAdd(url, spiel);

        return spiel;
    }
}
