using System.Net;
using System.Windows.Input;

namespace FinancialTracker.ViewModels;

public class DownloadConnectingViewModel : ViewModelBase {
    public IPAddress IpAddress { get; }
    public ICommand CancelCommand { get; }

    public DownloadConnectingViewModel(IPAddress ipAddress, ICommand cancelCommand) {
        IpAddress = ipAddress;
        CancelCommand = cancelCommand;
    }
}
