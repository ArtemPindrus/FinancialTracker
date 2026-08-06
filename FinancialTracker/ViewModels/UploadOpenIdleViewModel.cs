using System;
using System.Windows.Input;

namespace FinancialTracker.ViewModels;

public class UploadOpenIdleViewModel : UploadOpenViewModel {
    public UploadOpenIdleViewModel(ICommand stopServerCommand) : base(stopServerCommand) {
    }
}
