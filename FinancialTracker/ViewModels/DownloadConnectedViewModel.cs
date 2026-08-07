using System;
using System.Net;
using System.Windows.Input;

namespace FinancialTracker.ViewModels;

public class DownloadConnectedViewModel : ViewModelBase {
    public IPAddress IpAddress { get; }
    public ICommand DisconnectCommand { get; }

    public DownloadConnectedViewModel(IPAddress ipAddress, ICommand disconnectCommand) {
        IpAddress = ipAddress;
        DisconnectCommand = disconnectCommand;
    }
}
