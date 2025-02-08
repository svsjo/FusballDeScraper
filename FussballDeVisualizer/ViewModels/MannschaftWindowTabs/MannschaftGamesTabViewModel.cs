using FusballDeScraper.Datenklassen;
using FussballDeVisualizer.Helper;
using FussballDeVisualizer.Views.MainWindowTabs;

namespace FussballDeVisualizer.ViewModels.MannschaftWindowTabs;

public class MannschaftGamesTabViewModel : TabViewModel
{
    public override object View => new FairnessTableTabView();
    public override string Title => "Mannschaft Spiele";

    private Liga _liga;
    public MannschaftGamesTabViewModel(Liga liga)
    {
        _liga = liga;
    }
}
