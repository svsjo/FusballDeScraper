using FusballDeScraper;

public class Program
{
    public Program() 
    {
        Datenhaltung = new Datenhaltung();
        WebExtraktor = new WebExtraktor();
    }
    public Datenhaltung? Datenhaltung { get; set; }
    public WebExtraktor? WebExtraktor { get; set; }

    public static void Main(string[] args)
    {
        // Get Data
        // Do Filters
        // Display Results
    }
}
