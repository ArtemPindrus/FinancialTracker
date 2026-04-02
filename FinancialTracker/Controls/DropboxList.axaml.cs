using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancialTracker.Controls;

public partial class DropboxList : UserControl {
    public static readonly StyledProperty<List<string>> ComboBoxItemsProperty =
        AvaloniaProperty.Register<DropboxList, List<string>>(nameof(ComboBoxItems));

    public static readonly StyledProperty<List<string>> InitialySelectedProperty =
        AvaloniaProperty.Register<DropboxList, List<string>>(nameof(InitialySelected));

    public List<string> ComboBoxItems {
        get => GetValue(ComboBoxItemsProperty);
        set => SetValue(ComboBoxItemsProperty, value);
    }

    public List<string> InitialySelected {
        get => GetValue(InitialySelectedProperty);
        set => SetValue(InitialySelectedProperty, value);
    }

    public List<string> SelectedItems {
        get {
            List<string> selected = [];
            foreach (ComboBox c in Wrap.Children.Cast<ComboBox>()) {
                if (c.SelectedItem is string s) selected.Add(s);
            }
            return selected;
        }
    }

    public DropboxList() {
        InitializeComponent();
    }

    private void UserControl_Initialized(object? sender, System.EventArgs e) {
        foreach (var i in InitialySelected) {
            AddComboBox(i);
        }

        AddEmptyComboBox();
    }

    private void Empty_SelectionChanged(object? sender, SelectionChangedEventArgs e) {
        ComboBox? c = (ComboBox)sender;

        c.SelectionChanged -= Empty_SelectionChanged;
        AddEmptyComboBox();
    }

    private void AddEmptyComboBox() {
        var empty = AddComboBox(null);
        empty.SelectionChanged += Empty_SelectionChanged;
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

        return c;
    }
}