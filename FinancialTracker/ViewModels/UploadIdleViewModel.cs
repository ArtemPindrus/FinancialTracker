using System;
using System.Windows.Input;

namespace FinancialTracker.ViewModels;

public class UploadIdleViewModel : ViewModelBase {
    public ICommand StartServerCommand { get; }

    public UploadIdleViewModel(ICommand startServerCommand) {
        StartServerCommand = startServerCommand;
    }
}
