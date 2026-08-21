using CommunityToolkit.Mvvm.Input;
using FinancialTracker.Models;
using System.Collections.Generic;
using System.Windows.Input;

namespace FinancialTracker.ViewModels;

public class SimpleButtonViewModel {
    public ICommand Command { get; }
    public string Text { get; }
    public string? Tooltip { get; init; }

    public SimpleButtonViewModel(ICommand command, string text) {
        Command = command;
        Text = text;
    }
}

public partial class FinanceUpdateViewModel : FinanceEditViewModel {
    public FinanceUpdateViewModel(FinanceRecordDto record, List<string> availableTags) : base(record, availableTags) {
        Buttons.Add(new(SaveCommand, "Save") { Tooltip = "Save changes to the record." });
        Buttons.Add(new(RevertCommand, "Revert") { Tooltip = "Revert record changes." });
    }

    [RelayCommand]
    void Save() {
        Flush();
    }

    [RelayCommand]
    void Revert() {
        Pull();
    }
}
