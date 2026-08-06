using System;
using System.Windows.Input;

namespace FinancialTracker.ViewModels;

public class MainNavigationUnsafePopupViewModel : ViewModelBase {
    public string Message { get; }
    public ICommand CancelCommand { get; }
    public ICommand ContinueCommand { get; }

    public MainNavigationUnsafePopupViewModel(string message, ICommand cancelCommand, ICommand continueCommand) {
        Message = message;
        CancelCommand = cancelCommand;
        ContinueCommand = continueCommand;
    }
}
