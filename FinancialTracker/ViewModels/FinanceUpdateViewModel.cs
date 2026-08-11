using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
    public partial string? TagTextBox { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> Tags { get; set; }

    public List<string> SelectedTags { get; set; } = [];

    public List<string> AvailableTags { get; }

    public FinanceUpdateViewModel(FinanceRecordDto record, List<string> availableTags) {
        Name = record.Name;
        Amount = record.Amount;
        Date = record.Date;
        Tags = new(record.Tags);
        IsDeleted = record.IsDeleted;
        AvailableTags = availableTags;

        this.record = record;
    }

    [RelayCommand]
    void AddTag(string tag) {
        Tags.Add(tag);

        TagTextBox = "";
    }

    [RelayCommand]
    void DeleteTags(IEnumerable<string> tags) {
        foreach (var t in tags.ToArray()) {
            DeleteTag(t);
        }
    }

    [RelayCommand]
    void DeleteTag(string tag) {
        Tags.Remove(tag);
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
