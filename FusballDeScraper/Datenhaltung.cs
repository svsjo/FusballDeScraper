using FusballDeScraper.Datenextrahierung;
using FusballDeScraper.Datenklassen;
using FusballDeScraper.Datenklassen.Helper;
using System;
using System.Text;
using System.Text.Json;

namespace FusballDeScraper;

public static class Datenhaltung
{
    public static Liga? Liga { get; set; } = default;
    public static DateTime? LastUpdated { get; set; } = DateTime.Now;
    public static bool IsInitialized { get; set; } = false;

    public static async Task Initialize(string ligaUrl, IProgress<string> progress)
    {
        Liga = new();

        Liga.Name = SpielklasseExtractor.ExtractSpielklasse(ligaUrl);

        progress.Report("Lade Tabelle...");
        Liga.Tabelle = await TabelleExtractor.GetTabelle(ligaUrl);

        progress.Report("Lade Tabellenmatrix...");
        Liga.TabellenMatrix = PunkteMatrixRechner.BerechnePunkteMatrix(Liga.Tabelle);

        progress.Report("Lade Torschützen...");
        var urlScorer = ligaUrl.Replace("spieltagsuebersicht", "torjaeger") + "section/top-scorer";
        Liga.Torschuetzen = await TorschuetzenExtractor.GetTorschuetzen(urlScorer, Liga);

        progress.Report("Lade Mannschaften...");
        var mannschaftsUrls = Liga.Tabelle
            .Select(x => x.UrlMannschaft)
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();

        foreach(var mannschaft in mannschaftsUrls)
        {
            if (mannschaft == default) continue;

            Liga.Mannschaften.Add(await MannschaftExtractor.GetMannschaft(mannschaft, Liga, progress));
        }

        Liga.UpdateAktuellerSpieltag();

        progress.Report("Fertig!");

        LastUpdated = DateTime.Now;
        IsInitialized = true;
    }

    
    private static string _path = "data.json";

    public static void SaveResultsJson()
    {
        if (Liga == default) return;

        var jsonString = JsonSerializer.Serialize<Liga>(Liga);
        System.IO.File.WriteAllText(_path, jsonString);
    }

    public static void LoadSavedJsonResults()
    {
        var jsonString = System.IO.File.ReadAllText(_path);
        Liga = JsonSerializer.Deserialize<Liga>(jsonString);
    }
}