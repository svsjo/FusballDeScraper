using FusballDeScraper.Datenklassen;
using FussballDeVisualizer.Helper;
using FussballDeVisualizer.Views.MainWindowTabs;

namespace FussballDeVisualizer.ViewModels.MainWindowTabs;

public class ScorerTabViewModel : TabViewModel
{
    public override object View => new ScorerTabView();
    public override string Title => "Scorer";

    private Liga? _liga;
    public Liga? Liga
    {
        get => _liga;
        set
        {
            _liga = value;
            OnPropertyChanged(nameof(Liga));
        }
    }
    public void SetLiga(Liga liga) { Liga = liga; }
}
