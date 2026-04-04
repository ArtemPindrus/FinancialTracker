using Avalonia;
using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace FinancialTracker.Controls;

public partial class DropboxList : UserControl {
    private readonly ComboBoxesList comboBoxesList;

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

        comboBoxesList = new(Wrap);
    }

    private void UserControl_Initialized(object? sender, EventArgs e) {
        InitializeComboBoxes();

        if (Selected is ObservableCollection<string> observableSelectedNew) {
            observableSelectedNew.CollectionChanged += Selected_CollectionChanged;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedProperty && ComboBoxItems is not null) {
            object? oldValue = change.OldValue;

            if (oldValue is ObservableCollection<string> selectedOld) {
                selectedOld.CollectionChanged -= Selected_CollectionChanged;
            }

            InitializeComboBoxes();

            if (Selected is ObservableCollection<string> selectedNew) {
                selectedNew.CollectionChanged += Selected_CollectionChanged;
            }
        }
    }

    private void Selected_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        InitializeComboBoxes();
    }

    private void InitializeComboBoxes() {
        if (!ComboBoxItems.Contains(null)) ComboBoxItems.Insert(0, null);

        for (int i = Selected.Count; i < comboBoxesList.Count; i++) {
            FAComboBox c = comboBoxesList[i];
            c.IsVisible = false;
        }

        int boxesLeft = comboBoxesList.Count;
        for (int i = 0; i < Selected.Count; i++) {
            string? s = Selected[i];

            if (s is null) break;

            if (boxesLeft > 0) {
                FAComboBox c = comboBoxesList[i];
                c.IsVisible = true;

                c.SelectionChanged -= ComboBox_SelectionChanged;
                c.SelectedIndex = ComboBoxItems.IndexOf(s);
                c.SelectionChanged += ComboBox_SelectionChanged;

                boxesLeft--;
            } else {
                AddComboBox(s);
            }
        }

        if (boxesLeft == 0) AddComboBox(null);
        else {
            FAComboBox c = comboBoxesList[Selected.Count];
            c.IsVisible = true;
            c.SelectedIndex = 0;
        }
    }

    private void AddComboBox(string? selected) {
        FAComboBox c = new() {
            ItemsSource = ComboBoxItems,
            IsTextSearchEnabled = true,
        };

        if (selected is not null) {
            if (!ComboBoxItems.Contains(selected)) throw new ArgumentException($"Selected was not in the possible values.");

            c.SelectedIndex = ComboBoxItems.IndexOf(selected);
        }

        comboBoxesList.AddComboBox(c);

        c.SelectionChanged += ComboBox_SelectionChanged;
    }

    private void ComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e) {
        if (sender is not FAComboBox c) return;
                
        int cInd = comboBoxesList.IndexOf(c);

        string? s = (string?)e.AddedItems[0];
        
        if (Selected.Count > cInd) {
            if (s is null) Selected.RemoveAt(cInd);
            else Selected[cInd] = s;
        } else if(s is not null) Selected.Add(s);
    }
}

public class ComboBoxesList : IEnumerable<FAComboBox> {
    private readonly Avalonia.Controls.Controls boxes;

    public FAComboBox this[int index] {
        get => (FAComboBox)boxes[index];
        set => boxes[index] = value;
    }

    public int Count => boxes.Count;

    public ComboBoxesList(WrapPanel wrap) {
        boxes = wrap.Children;
    }

    public void AddComboBox(FAComboBox comboBox) => boxes.Add(comboBox);

    public void Clear() => boxes.Clear();

    public int IndexOf(FAComboBox comboBox) => boxes.IndexOf(comboBox);

    public IEnumerator<FAComboBox> GetEnumerator() => boxes.Cast<FAComboBox>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}