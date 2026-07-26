using CommunityToolkit.Mvvm.ComponentModel;
using FinancialTracker.DataAccessLayer.Services;
using FinancialTracker.StateMachines;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;

namespace FinancialTracker.ViewModels {
    public partial class DownloadViewModel : ViewModelBase {
        private readonly SyncClient syncClient;

        [ObservableProperty]
        object? currentViewModel;

        public DownloadViewModel(SyncClient syncClient) {
            this.syncClient = syncClient;
            this.syncClient.Start();

            this.syncClient.PropertyChanged += SyncServer_PropertyChanged;
        }

        private void SyncServer_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(SyncClient.stateId)) {
                SyncUiToSmState();
            }
        }

        private void SyncUiToSmState() {
            CurrentViewModel = syncClient.stateId switch {
                
                _ => null
            };
        }
    }
}
