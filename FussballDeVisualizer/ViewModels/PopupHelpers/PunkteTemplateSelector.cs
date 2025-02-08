using System.Windows.Controls;
using System.Windows;
using System.Collections.Generic;

namespace FussballDeVisualizer.ViewModels.PopupHelpers;

public class PunkteTemplateSelector : DataTemplateSelector
{
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is KeyValuePair<string, int> punktedifferenz)
        {
            // Logik: Basierend auf dem Key (Spaltenname) und Value (Punktewert).
            if (punktedifferenz.Value >= 0)
            {
                return PositiveTemplate; // Template für positive Punktedifferenzen
            }
            else
            {
                return NegativeTemplate; // Template für negative Punktedifferenzen
            }
        }

        return base.SelectTemplate(item, container);
    }

    // Templates für positive und negative Werte
    public DataTemplate PositiveTemplate { get; set; }
    public DataTemplate NegativeTemplate { get; set; }
}