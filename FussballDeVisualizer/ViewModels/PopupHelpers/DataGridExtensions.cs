using System.Collections;
using System.Windows.Controls;
using System.Windows;
using FusballDeScraper.Datenextrahierung;

namespace FussballDeVisualizer.ViewModels.PopupHelpers;

public static class DataGridExtensions
{
    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.RegisterAttached(
            "SelectedItems",
            typeof(IList),
            typeof(DataGridExtensions),
            new PropertyMetadata(null, OnSelectedItemsChanged));

    public static void SetSelectedItems(DataGrid dataGrid, IList value)
    {
        dataGrid.SetValue(SelectedItemsProperty, value);
    }

    public static IList GetSelectedItems(DataGrid dataGrid)
    {
        return (IList)dataGrid.GetValue(SelectedItemsProperty);
    }

    private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DataGrid dataGrid)
        {
            dataGrid.SelectionChanged += (s, ev) =>
            {
                var selectedItems = GetSelectedItems(dataGrid);
                selectedItems?.Clear();
                foreach (var item in dataGrid.SelectedItems)
                {
                    selectedItems?.Add(item);
                }
            };
        }
    }
}
