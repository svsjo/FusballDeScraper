using FusballDeScraper.Datenklassen;
using FussballDeVisualizer.Helper;
using FussballDeVisualizer.ViewModels.MainWindowTabs;
using System.Collections.ObjectModel;

namespace FussballDeVisualizer.ViewModels.MannschaftWindowTabs;

public class MannschaftMainWindowViewModel : BaseViewModel
{
    public override string Title => "MainMannschaft";

    public ObservableCollection<TabViewModel> Tabs { get; set; }
    public TabViewModel SelectedTab { get; set; }

    public MannschaftMainWindowViewModel(Liga liga)
    {
        Tabs = new ObservableCollection<TabViewModel>
        {
            new MannschaftTableTabViewModel(liga),
            new MannschaftEinsatzTabViewModel(liga),
            new MannschaftGamesTabViewModel(liga),
            new MannschaftAnalysenTabViewModel(liga),
        };

        SelectedTab = Tabs[0];
    }
}
