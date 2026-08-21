using CommunityToolkit.Mvvm.Input;
using FinancialTracker.Models;
using System;
using System.Collections.Generic;

namespace FinancialTracker.ViewModels;

public partial class FinanceAddViewModel : FinanceEditViewModel {
    public FinanceAddViewModel(List<string> availableTags, Action<FinanceRecordDto> addAction) : base(new(), availableTags) {
        Buttons.Add(new(new RelayCommand(() => addAction(Materialize())), "Add") { Tooltip = "Add record." });
    }
}
