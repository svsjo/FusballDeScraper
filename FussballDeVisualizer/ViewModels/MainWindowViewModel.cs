using FusballDeScraper;
using FussballDeVisualizer.Datenklassen;
using FussballDeVisualizer.Helper;
using FussballDeVisualizer.ViewModels.MainWindowTabs;
using FussballDeVisualizer.Views;
using FussballDeVisualizer.Views.MainWindowTabs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace FussballDeVisualizer.ViewModels;

public class MainViewModel : BaseViewModel
{
    public override string Title => "Main";

    public ObservableCollection<TabViewModel> Tabs { get; set; }
    public TabViewModel SelectedTab { get; set; }

    public MainViewModel()
    {
        var configVm = new ConfigTabViewModel();
        var tableVm = new TableTabViewModel();
        var fairnessVm = new FairnessTableTabViewModel();
        var gameDayVm = new GamedayTabViewModel();
        var scorerVm = new ScorerTabViewModel();

        Tabs = new ObservableCollection<TabViewModel>
        {
            configVm,
            tableVm,
            fairnessVm,
            gameDayVm,
            scorerVm,
        };
        
        SelectedTab = Tabs[0];

        configVm.StartedExtractionEventHandler += (sender, obj) => 
        {
            tableVm.SetLiga(configVm.Liga);
            fairnessVm.SetLiga(configVm.Liga);
            gameDayVm.SetLiga(configVm.Liga);
            scorerVm.SetLiga(configVm.Liga);
        };

        configVm.TableFinishedEventHandler += (sender, obj) => { tableVm.UpdateTableMatrix(); };
    }
}
