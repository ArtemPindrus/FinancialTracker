using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancialTracker.Controls;

public partial class DropboxList : UserControl {
    public static readonly StyledProperty<List<string?>> ComboBoxItemsProperty =
        AvaloniaProperty.Register<DropboxList, List<string?>>(nameof(ComboBoxItems));

    public static readonly StyledProperty<List<string>> SelectedProperty =
        AvaloniaProperty.Register<DropboxList, List<string>>(nameof(Selected));

    public List<string?> ComboBoxItems {
        get => GetValue(ComboBoxItemsProperty);
        set => SetValue(ComboBoxItemsProperty, value);
    }

    public List<string> Selected {
        get => GetValue(SelectedProperty);
        set => SetValue(SelectedProperty, value);
    }

    public DropboxList() {
        InitializeComponent();
    }

    private void UserControl_Initialized(object? sender, EventArgs e) {
        foreach (var i in Selected) {
            AddComboBox(i);
        }

        AddComboBox(null);
    }

    private void ComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e) {
        ComboBox? c = (ComboBox)sender;

        string? s = (string?)e.AddedItems[0];
        if (s == null) {
            int cInd = Wrap.Children.IndexOf(c);

            if (cInd < Wrap.Children.Count - 1) {
                Selected.RemoveAt(cInd);
                Wrap.Children.RemoveAt(cInd);
            }
        } else {
            int cInd = Wrap.Children.IndexOf(c);

            if (cInd == Wrap.Children.Count - 1) {
                Selected.Add(s);
                AddComboBox(null);
            } else {
                Selected[cInd] = s;
            }
        }
    }

    private ComboBox AddComboBox(string? selected) {
        ComboBox c = new() {
            ItemsSource = ComboBoxItems
        };

        if (selected is not null) {
            if (!ComboBoxItems.Contains(selected)) throw new ArgumentException($"Selected was not in the possible values.");

            c.SelectedIndex = ComboBoxItems.IndexOf(selected);
        }

        Wrap.Children.Add(c);

        c.SelectionChanged += ComboBox_SelectionChanged;

        return c;
    }
}