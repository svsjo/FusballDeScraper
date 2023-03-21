using HtmlAgilityPack;

namespace FusballDeScraper;

public class WebExtraktor
{
    public async Task<Liga> GetLiga(string Url)
    {
        /* Get HTML Document */
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(Url);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var divs = htmlDocument.DocumentNode.Descendants("div");

        var liga = new Liga(Url);

        liga.Mannschaften = GetMannschaften();
        liga.Tabelle = GetTabelle();
        liga.Torschuetzen = GetTorschuetzen();

        return liga;
    }

    private List<VereinPlatzierung> GetTabelle()
    {
        var tabelle = new List<VereinPlatzierung>();

        return tabelle;
    }

    private List<TorschuetzenEintrag> GetTorschuetzen()
    {
        var alleTorschuetzen = new List<TorschuetzenEintrag>();

        return alleTorschuetzen;
    }

    private List<Mannschaft> GetMannschaften()
    {
        var mannschaften = new List<Mannschaft>();

        // foreach ... GetMannschaft
        
        return mannschaften;
    }

    private Mannschaft GetMannschaft(string Url)
    {
        var mannschaft = new Mannschaft();

        // foreach ... GetSpiel
        // foreach ... GetAbgeschlossenesSpiel
        // foreach ... GetEinsatze hier drin lokal
        
        return mannschaft;
    }

    private Spiel GetSpiel(string Url)
    {
        var spiel = new Spiel();

        return spiel;
    }

    private AbgeschlossenesSpiel getAbgeschlossenesSpiel(string Url)
    {
        var abgeschlossenesSpiel = new AbgeschlossenesSpiel();

        return abgeschlossenesSpiel;
    }
}