using FusballDeScraper.Datenklassen;
using FussballDeVisualizer.Helper;
using FussballDeVisualizer.Views.MainWindowTabs;

namespace FussballDeVisualizer.ViewModels.MannschaftWindowTabs;

public class MannschaftAnalysenTabViewModel : TabViewModel
{
    public override object View => new FairnessTableTabView();
    public override string Title => "Mannschaft Analysen";

    private Liga _liga;
    public MannschaftAnalysenTabViewModel(Liga liga) 
    { 
        _liga = liga; 
    }
}
