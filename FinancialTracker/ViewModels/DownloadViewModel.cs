using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialTracker.Legacy;
using FinancialTracker.StateMachines;
using HarfBuzzSharp;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FinancialTracker.ViewModels {
    public partial class DownloadViewModel : ViewModelBase {
        private readonly SyncClient syncClient;

        [ObservableProperty]
        object? currentViewModel;

        public DownloadViewModel(IConfiguration config) {
            syncClient = new(config);
            syncClient.Start();

            syncClient.PropertyChanged += SyncServer_PropertyChanged;
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
