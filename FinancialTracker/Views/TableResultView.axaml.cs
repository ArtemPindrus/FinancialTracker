using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using FinancialTracker.ViewModels;
using System;

namespace FinancialTracker.Views;

public partial class TableResultView : UserControl {
    public TableResultView() {
        InitializeComponent();
    }

    private void DataGrid_Initialized(object? sender, EventArgs e) {
        TableResultViewModel vm = (TableResultViewModel)DataContext;

        for (int i = 0; i < vm.Columns.Length; i++) {
            string column = vm.Columns[i];

            DataGridTextColumn textColumn = new() {
                Header = column,
                Binding = new Binding($"[{i}]")
            };

            DataGrid.Columns.Add(textColumn);
        }
    }
}