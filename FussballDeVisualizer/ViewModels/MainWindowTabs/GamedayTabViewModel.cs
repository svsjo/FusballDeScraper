using FusballDeScraper.Datenklassen;
using FussballDeVisualizer.Helper;
using FussballDeVisualizer.Views.MainWindowTabs;

namespace FussballDeVisualizer.ViewModels.MainWindowTabs;

public class GamedayTabViewModel : TabViewModel
{
    public override object View => new GamedayTabView();
    public override string Title => "Gameday";

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