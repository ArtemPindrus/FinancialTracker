using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FinancialTracker.ViewModels {
    public partial class UploadViewModel : ViewModelBase, IDisposable {
        private readonly SyncServer syncServer;

        public string ClientIp => syncServer.ClientIp ?? "Not connected";

        public bool IsConnected => syncServer.IsConnected;

        public bool IsRunning => syncServer.IsRunning;

        public string WifiIpAddress {
            get {
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces()) {
                    if (nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                        && nic.OperationalStatus == OperationalStatus.Up
                        && !nic.Description.Contains("Virtual", StringComparison.OrdinalIgnoreCase)) {
                        foreach (UnicastIPAddressInformation ip in nic.GetIPProperties().UnicastAddresses) {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork) {
                                return ip.Address.ToString();
                            }
                        }
                    }
                }

                return "Failed to find IP address of Wifi Adapter.";
            }
        }

        public UploadViewModel(SyncServer syncServer) {
            this.syncServer = syncServer;

            syncServer.PropertyChanged += SyncServer_PropertyChanged;
        }

        private void SyncServer_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(syncServer.IsRunning)) {
                OnPropertyChanged(nameof(syncServer.IsRunning));

                StartServerCommand.NotifyCanExecuteChanged();
                StopServerCommand.NotifyCanExecuteChanged();
            } else if (e.PropertyName == nameof(syncServer.IsConnected)) {
                OnPropertyChanged(nameof(syncServer.IsConnected));
            } else if (e.PropertyName == nameof(syncServer.ClientIp)) {
                OnPropertyChanged(nameof(syncServer.ClientIp));
            }
        }

        [RelayCommand(CanExecute = nameof(CanStartServerAsync))]
        private async Task StartServerAsync() {
            await syncServer.StartServerAsync();
            await syncServer.ConnectClient();
        }

        private bool CanStartServerAsync() => !IsRunning;

        [RelayCommand(CanExecute = nameof(CanStopServerAsync))]
        private async Task StopServerAsync() {
            await syncServer.StopServerAsync();
        }

        private bool CanStopServerAsync() => IsRunning;

        [RelayCommand]
        private async Task SendDatabaseAsync() {
            await syncServer.SendDatabase();
        }

        public void Dispose() {
            syncServer.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
