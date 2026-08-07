using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Net;

namespace FinancialTracker.ViewModels;

public partial class DownloadIdleViewModel : ViewModelBase {
    private readonly Action<IPAddress> connectRequest;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    public partial string? IpAddress { get; set; }

    public DownloadIdleViewModel(Action<IPAddress> connectRequest) {
        this.connectRequest = connectRequest;
    }

    [RelayCommand(CanExecute = nameof(CanConnect))]
    void Connect() {
        connectRequest(IPAddress.Parse(IpAddress));
    }

    bool CanConnect() {
        return IPAddress.TryParse(IpAddress, out _);
    }
}
