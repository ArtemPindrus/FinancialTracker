using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialTracker.StateMachines;
using System;
using System.ComponentModel;
using System.Net;

namespace FinancialTracker.ViewModels {
    public partial class DownloadViewModel : MainNavigationPaneViewModel, IDisposable {
        private readonly SyncClient syncClient;

        [ObservableProperty]
        public partial object? CurrentViewModel { get; private set; }

        public DownloadViewModel(SyncClient syncClient) {
            this.syncClient = syncClient;
            this.syncClient.Start();

            this.syncClient.PropertyChanged += SyncServer_PropertyChanged;

            SyncUiToSmState();
        }

        public void Dispose() {
            syncClient.Dispose();
        }

        private void SyncServer_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(SyncClient.stateId)) {
                SyncUiToSmState();
            }
        }

        private void SyncUiToSmState() {
            CurrentViewModel = syncClient.stateId switch {
                SyncClient.StateId.IDLE => new DownloadIdleViewModel(Connect),
                SyncClient.StateId.CONNECTING => new DownloadConnectingViewModel(syncClient.RequestedIpAddress, CancelConnectionCommand),
                SyncClient.StateId.CONNECTED => new DownloadConnectedViewModel(syncClient.ConnectedIpAddress, DisconnectCommand),
                SyncClient.StateId.RECEIVING => new DownloadReceivingViewModel(syncClient),
                _ => "UNKNOWN STATE"
            };
        }

        void Connect(IPAddress ipAddress) {
            syncClient.Connect(ipAddress);
        }

        [RelayCommand]
        void CancelConnection() {
            syncClient.DispatchEventNotify(SyncClient.EventId.CONNECTIONCANCELED);
        }

        [RelayCommand]
        void Disconnect() {
            syncClient.DispatchEventNotify(SyncClient.EventId.DISCONNECTED);
        }
    }
}
