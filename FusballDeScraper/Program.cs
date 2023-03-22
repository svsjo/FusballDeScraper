using System.Runtime.CompilerServices;
using FusballDeScraper;

public class Program
{
    public static void Main(string[] args)
    {
        var datenhaltung = new Datenhaltung();
        var webExtraktor = new WebExtraktor();

        const string url =
            @"https://www.fussball.de/spieltagsuebersicht/bezirksliga-herren-bezirk-offenburg-bezirksliga-herren-saison2223-suedbaden/-/staffel/02IKSBQ9U4000006VS5489B3VVETK79U-G#!/";

        datenhaltung.Liga = webExtraktor.GetLiga(url).Result;

        Console.WriteLine("Welchen Verein möchtest du analysieren? Hier die Optionen: \n");
        datenhaltung.Liga.Mannschaften.ForEach(x => Console.WriteLine(x.Name));
        Console.WriteLine();

        var mannschaft = Console.ReadLine();

        // Do Filters

        // Display Results
    }
}
