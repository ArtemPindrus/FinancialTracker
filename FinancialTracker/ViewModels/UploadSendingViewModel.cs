using System.Windows.Input;

namespace FinancialTracker.ViewModels;

public class UploadSendingViewModel : UploadConnectedViewModel {
    public UploadSendingViewModel(ICommand stopServerCommand, ICommand disconnectCommand, string ipAddress) : base(stopServerCommand, disconnectCommand, ipAddress) {
    }
}
