using System;

namespace FinancialTracker.ViewModels;

public class ProgressRingViewModel : ViewModelBase {
    public string Message { get; }

    public ProgressRingViewModel(string message) {
        Message = message;
    }
}
