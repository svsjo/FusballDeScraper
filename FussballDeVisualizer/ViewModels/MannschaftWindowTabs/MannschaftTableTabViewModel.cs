using FusballDeScraper.Datenklassen;
using FussballDeVisualizer.Helper;
using FussballDeVisualizer.Views.MainWindowTabs;

namespace FussballDeVisualizer.ViewModels.MannschaftWindowTabs;

public class MannschaftTableTabViewModel : TabViewModel
{
    public override object View => new FairnessTableTabView();
    public override string Title => "Mannschaft Table";

    private Liga _liga;
    public MannschaftTableTabViewModel(Liga liga)
    {
        _liga = liga;
    }
}