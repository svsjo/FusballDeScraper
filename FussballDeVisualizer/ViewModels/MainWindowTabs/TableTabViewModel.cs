using FusballDeScraper.Datenklassen;
using FusballDeScraper.Datenklassen.Tabelle;
using FussballDeVisualizer.Helper;
using FussballDeVisualizer.Views.MainWindowTabs;
using OpenTK.Compute.OpenCL;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FussballDeVisualizer.ViewModels.MainWindowTabs;

public class TableTabViewModel : TabViewModel
{
    public override object View => new TableTabView();
    public override string Title => "Table";

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

    private Grid _matrix;
    public Grid Matrix
    {
        get => _matrix;
        set
        {
            _matrix = value;
            OnPropertyChanged(nameof(Matrix));
        }
    }

    public TableTabViewModel()
    {
        Matrix = new Grid();
    }

    public ObservableCollection<VereinPlatzierung> SelectedItems { get; set; } = new ObservableCollection<VereinPlatzierung>(); // hier leer, da allgemein Tabelle

    public void SetLiga(Liga liga) { Liga = liga; }

    public void UpdateTableMatrix()
    {
        if (Liga?.TabellenMatrix == default) return;

        var count = 0;

        // Definitionen
        Matrix.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        Matrix.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        count++;

        foreach (var mannschaft in Liga.TabellenMatrix)
        {
            Matrix.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Matrix.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            count++;
        }

        // Ueberschriften
        for (var spalte = 1; spalte < count; spalte++)
        {
            var topHeader = new Border
            {
                BorderBrush = System.Windows.Media.Brushes.Black,
                BorderThickness = new Thickness(0.5),
                Margin = new Thickness(2),
                Child = new TextBlock
                {
                    Text = Liga.TabellenMatrix[spalte - 1].Mannschaft,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };

            Grid.SetRow(topHeader, 0);
            Grid.SetColumn(topHeader, spalte);
            Matrix.Children.Add(topHeader);
        }

        for (var zeile = 1; zeile < count; zeile++)
        {
            var leftHeader = new Border
            {
                BorderBrush = System.Windows.Media.Brushes.Black,
                BorderThickness = new Thickness(0.5),
                Margin = new Thickness(2),
                Child = new TextBlock
                {
                    Text = Liga.TabellenMatrix[zeile - 1].Mannschaft,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                }
            };

            Grid.SetRow(leftHeader, zeile);
            Grid.SetColumn(leftHeader, 0);
            Matrix.Children.Add(leftHeader);
        }

        for (var zeileIndex = 1; zeileIndex < count; zeileIndex++)
        {
            var zeile = Liga.TabellenMatrix[zeileIndex - 1];

            for (var spalteIndex = 1; spalteIndex < count; spalteIndex++)
            {
                var zelle = zeile.PunkteDifferenzen.Values.ElementAt(spalteIndex - 1);

                var cellBox = new TextBlock
                {
                    Text = zelle.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Grid.SetRow(cellBox, zeileIndex);
                Grid.SetColumn(cellBox, spalteIndex);
                Matrix.Children.Add(cellBox);

                cellBox.MouseDown += (s, e) => SelektiereZeileUndSpalte(cellBox);
            }
        }
    }

    private void SelektiereZeileUndSpalte(TextBlock cellBox)
    {
        var row = Grid.GetRow(cellBox);
        var col = Grid.GetColumn(cellBox);

        var topCell = Matrix.Children
                .Cast<UIElement>()
                .FirstOrDefault(c => Grid.GetRow(c) == 0 && Grid.GetColumn(c) == col);

        var leftCell = Matrix.Children
                .Cast<UIElement>()
                .FirstOrDefault(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == 0);

        var selectCell = Matrix.Children
                .Cast<UIElement>()
                .FirstOrDefault(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == col);

        if (topCell is Border borderTop)
        {
            if (borderTop.Background == System.Windows.Media.Brushes.LightBlue)
            {
                borderTop.Background = System.Windows.Media.Brushes.White; // Reset
            }
            else
            {
                borderTop.Background = System.Windows.Media.Brushes.LightBlue; // Hervorhebung
            }
        }

        if (leftCell is Border borderLeft)
        {
            if (borderLeft.Background == System.Windows.Media.Brushes.LightBlue)
            {
                borderLeft.Background = System.Windows.Media.Brushes.White; // Reset
            }
            else
            {
                borderLeft.Background = System.Windows.Media.Brushes.LightBlue; // Hervorhebung
            }
        }

        if (selectCell is TextBlock cellBlock)
        {
            if (cellBlock.Background == System.Windows.Media.Brushes.LightBlue)
            {
                cellBlock.Background = System.Windows.Media.Brushes.White; // Reset
            }
            else
            {
                cellBlock.Background = System.Windows.Media.Brushes.LightBlue; // Hervorhebung
            }
        }
    }
}
