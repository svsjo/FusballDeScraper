using FusballDeScraper.Datenklassen;
using FussballDeVisualizer.Helper;
using FussballDeVisualizer.Views.MainWindowTabs;

namespace FussballDeVisualizer.ViewModels.MannschaftWindowTabs;

public class MannschaftEinsatzTabViewModel : TabViewModel
{
    public override object View => new FairnessTableTabView();
    public override string Title => "Mannschaft Einsatz";

    private Liga _liga;
    public MannschaftEinsatzTabViewModel(Liga liga)
    {
        _liga = liga;
    }
}
