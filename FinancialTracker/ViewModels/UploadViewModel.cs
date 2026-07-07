using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialTracker.StateMachines;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace FinancialTracker.ViewModels {
    public partial class UploadViewModel : ViewModelBase, IDisposable {
        private readonly SyncServer syncServer;

        [ObservableProperty]
        object? currentViewModel;

        public string ClientIp => syncServer.ClientIp ?? "Not connected";

        public int Port => SyncServer.Port;

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

        public UploadViewModel(IConfiguration config) {
            syncServer = new(config);
            syncServer.StartServer();
            syncServer.Start();

            syncServer.PropertyChanged += SyncServer_PropertyChanged;

            CurrentViewModel = new UploadDisconnectedViewModel(TryConnectingCommand);
        }

        public void Dispose() {
            syncServer.PropertyChanged -= SyncServer_PropertyChanged;

            syncServer.Dispose();
        }

        private void SyncServer_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(SyncServer.CurrentStateId)) {
                SyncUiToSmState();
            }
        }

        void SyncUiToSmState() {
            CurrentViewModel = syncServer.CurrentStateId switch {
                SyncServer.StateId.DISCONNECTED => new UploadDisconnectedViewModel(TryConnectingCommand),
                SyncServer.StateId.CONNECTING => new UploadConnectingViewModel(CancelConnectionCommand),
                SyncServer.StateId.CONNECTEDIDLE => new UploadConnectedViewModel(syncServer.ClientIp, DisconnectCommand, SendCommand),
                SyncServer.StateId.SENDING => "SENDING...",
                _ => null
            };
        }

        [RelayCommand]
        void Send() {
            syncServer.DispatchEvent(SyncServer.EventId.SEND);
        }

        [RelayCommand]
        void Disconnect() {
            syncServer.DispatchEvent(SyncServer.EventId.DISCONNECT);
        }

        [RelayCommand]
        void CancelConnection() {
            syncServer.DispatchEvent(SyncServer.EventId.CONNECTIONFAILED);
        }

        [RelayCommand]
        void TryConnecting() {
            syncServer.DispatchEvent(SyncServer.EventId.CONNECT);
        }
    }
}
