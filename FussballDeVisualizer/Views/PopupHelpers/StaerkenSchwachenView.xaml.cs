using FussballDeVisualizer.ViewModels;
using FussballDeVisualizer.ViewModels.PopupHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FussballDeVisualizer.Views
{
    /// <summary>
    /// Interaktionslogik für StaerkenSchwachenView.xaml
    /// </summary>
    public partial class StaerkenSchwachenView : Window
    {
        public StaerkenSchwachenView(StaerkenSchwaechenViewModel vm)
        {
            InitializeComponent();

            DataContext = vm;

            vm.GeneratePlot(PlotHome, StaerkenSchwaechenViewModel.PlotType.Home);
            vm.GeneratePlot(PlotAway, StaerkenSchwaechenViewModel.PlotType.Away);
            vm.GeneratePlot(PlotHomeStrength, StaerkenSchwaechenViewModel.PlotType.HomeStrengths);
            vm.GeneratePlot(PlotAwayStrength, StaerkenSchwaechenViewModel.PlotType.AwayStrengths);
        }
    }
}
