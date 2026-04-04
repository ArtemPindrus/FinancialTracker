using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace FinancialTracker.Controls;

public partial class DropboxList : UserControl, IDisposable {
    public static readonly StyledProperty<List<string?>> ComboBoxItemsProperty =
        AvaloniaProperty.Register<DropboxList, List<string?>>(nameof(ComboBoxItems));

    public static readonly StyledProperty<IList<string>> SelectedProperty =
        AvaloniaProperty.Register<DropboxList, IList<string>>(nameof(Selected));

    public List<string?> ComboBoxItems {
        get => GetValue(ComboBoxItemsProperty);
        set => SetValue(ComboBoxItemsProperty, value);
    }

    public IList<string> Selected {
        get => GetValue(SelectedProperty);
        set => SetValue(SelectedProperty, value);
    }

    public DropboxList() {
        InitializeComponent();
    }

    private void UserControl_Initialized(object? sender, EventArgs e) {
        InitializeComboBoxes();

        if (Selected is ObservableCollection<string> observableSelectedNew) {
            observableSelectedNew.CollectionChanged += ObservableSelected_CollectionChanged;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedProperty && ComboBoxItems is not null) {
            object? oldValue = change.OldValue;

            if (oldValue is ObservableCollection<string> observableSelectedOld) {
                observableSelectedOld.CollectionChanged -= ObservableSelected_CollectionChanged;
            }

            InitializeComboBoxes();

            if (Selected is ObservableCollection<string> observableSelectedNew) {
                observableSelectedNew.CollectionChanged += ObservableSelected_CollectionChanged;
            }
        }
    }

    private void ObservableSelected_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        InitializeComboBoxes();
    }

    private void InitializeComboBoxes() {
        if (!ComboBoxItems.Contains(null)) ComboBoxItems.Insert(0, null);

        DisposeOfWrapChildren();

        foreach (var i in Selected) {
            AddComboBox(i);
        }

        AddComboBox(null);
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

    private void ComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e) {
        ComboBox? c = (ComboBox)sender;

        string? s = (string?)e.AddedItems[0];
        if (s == null) {
            int cInd = Wrap.Children.IndexOf(c);

            if (cInd < Wrap.Children.Count - 1) {
                Selected.RemoveAt(cInd);
            }
        } else {
            int cInd = Wrap.Children.IndexOf(c);

            if (cInd == Wrap.Children.Count - 1) {
                Selected.Add(s);
            } else {
                Selected[cInd] = s;
            }
        }
    }

    private void DisposeOfWrapChildren() {
        foreach (var c in Wrap.Children.Cast<ComboBox>()) {
            c.SelectionChanged -= ComboBox_SelectionChanged;
        }

        Wrap.Children.Clear();
    }


    public void Dispose() {
        foreach (var c in Wrap.Children.Cast<ComboBox>()) {
            c.SelectionChanged -= ComboBox_SelectionChanged;
        }
    }
}