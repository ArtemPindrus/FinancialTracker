using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialTracker.StateMachines;
using FinancialTracker.Views;
using System;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace FinancialTracker.ViewModels {
    public partial class UploadViewModel : MainNavigationPaneViewModel, IDisposable {
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

        public UploadViewModel(SyncServer syncServerSm) {
            syncServer = syncServerSm;
            syncServerSm.Start();

            syncServerSm.PropertyChanged += SyncServer_PropertyChanged;
            SyncUiToSmState();
        }

        public void Dispose() {
            syncServer.PropertyChanged -= SyncServer_PropertyChanged;

            syncServer.Dispose();
        }

        private void SyncServer_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(SyncServer.stateId)) {
                SyncUiToSmState();
            }
        }

        void SyncUiToSmState() {
            CurrentViewModel = syncServer.stateId switch {
                SyncServer.StateId.IDLE => new UploadIdleViewModel(StartServerCommand),
                SyncServer.StateId.OPENIDLE => new UploadOpenIdleViewModel(StopServerCommand),
                SyncServer.StateId.CONNECTIONREQUESTED => new UploadConnectionRequestedViewModel(StopServerCommand,
                    AcceptConnectionCommand,
                    RejectConnectionCommand,
                    syncServer.ClientIp ?? throw new Exception("Client IP expected.")
                ),
                SyncServer.StateId.CONNECTEDIDLE => new UploadConnectedIdleViewModel(StopServerCommand,
                    DisconnectCommand,
                    SendCommand,
                    syncServer.ClientIp ?? throw new Exception("Client IP expected.")
                ),
                SyncServer.StateId.SENDING => new UploadSendingViewModel(StopServerCommand,
                    DisconnectCommand,
                    syncServer.ClientIp ?? throw new Exception("Client IP expected.")
                ),
                _ => "UNKNOWN STATE"
            };
        }

        [RelayCommand]
        void StartServer() {
            syncServer.DispatchEventNotify(SyncServer.EventId.STARTSERVER);
        }

        [RelayCommand]
        void StopServer() {
            syncServer.DispatchEventNotify(SyncServer.EventId.CLOSESERVER);
        }

        [RelayCommand]
        void AcceptConnection() {
            syncServer.DispatchEventNotify(SyncServer.EventId.CONNECTIONACCEPTED);
        }

        [RelayCommand]
        void RejectConnection() {
            syncServer.DispatchEventNotify(SyncServer.EventId.CONNECTIONREJECTED);
        }

        [RelayCommand]
        void Disconnect() {
            syncServer.DispatchEventNotify(SyncServer.EventId.DISCONNECTED);
        }

        [RelayCommand]
        void Send() {
            syncServer.DispatchEventNotify(SyncServer.EventId.SENDREQUEST);
        }
    }
}
