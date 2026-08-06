using System.Windows.Input;

namespace FinancialTracker.ViewModels;

public class UploadConnectedIdleViewModel : UploadConnectedViewModel {
    public ICommand SendCommand { get; }

    public UploadConnectedIdleViewModel(ICommand stopServerCommand,
        ICommand disconnectCommand, 
        ICommand sendCommand, 
        string ipAddress) : base(stopServerCommand, disconnectCommand, ipAddress) {
        SendCommand = sendCommand;
    }
}
