using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialTracker.Models;
using System;
using System.Collections.Generic;

namespace FinancialTracker.ViewModels;

public partial class FinanceUpdateViewModel : ViewModelBase {
    private readonly FinanceRecordDto record;

    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial double Amount { get; set; }

    [ObservableProperty]
    public partial DateOnly Date { get; set; }

    [ObservableProperty]
    public partial bool IsDeleted { get; set; }

    [ObservableProperty]
    public partial List<string> Tags { get; set; }

    public FinanceUpdateViewModel(FinanceRecordDto record) {
        Name = record.Name;
        Amount = record.Amount;
        Date = record.Date;
        Tags = [.. record.Tags];
        IsDeleted = record.IsDeleted;
        this.record = record;
    }

    [RelayCommand]
    void Save() {
        record.Name = Name;
        record.Amount = Amount;
        record.Date = Date;
        record.Tags = new(Tags);
        record.IsDeleted = IsDeleted;
    }

    [RelayCommand]
    void Revert() {
        Name = record.Name;
        Amount = record.Amount;
        Date = record.Date;
        Tags = [.. record.Tags];
        IsDeleted = record.IsDeleted;
    }
}
