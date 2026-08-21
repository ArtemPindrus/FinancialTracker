using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FinancialTracker.ViewModels;

public abstract partial class FinanceEditViewModel : ViewModelBase {
    private readonly FinanceRecordDto record;

    public int Id { get; }

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

    public List<SimpleButtonViewModel> Buttons { get; } = [];

    public FinanceEditViewModel(FinanceRecordDto record, List<string> availableTags) {
        Id = record.Id;
        Name = record.Name;
        Amount = record.Amount;
        Date = record.Date;
        Tags = new(record.Tags);
        IsDeleted = record.IsDeleted;
        AvailableTags = availableTags;

        this.record = record;
    }

    /// <summary>
    /// Materializes VM properties into a record.
    /// </summary>
    /// <returns></returns>
    public FinanceRecordDto Materialize() {
        return new(Id, Name, Amount, Date, Tags, true) { 
            IsDeleted = IsDeleted,
        };
    }

    /// <summary>
    /// Flushes VM properties into a record.
    /// </summary>
    /// <param name="record"></param>
    public void Flush() {
        record.Name = Name;
        record.Amount = Amount;
        record.Date = Date;
        record.Tags = new(Tags);
        record.IsDeleted = IsDeleted;
    }

    /// <summary>
    /// Pulls record properties into this VM.
    /// </summary>
    public void Pull() {
        Name = record.Name;
        Amount = record.Amount;
        Date = record.Date;
        Tags = [.. record.Tags];
        IsDeleted = record.IsDeleted;
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
}
