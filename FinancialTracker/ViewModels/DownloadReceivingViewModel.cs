using FinancialTracker.StateMachines;
using System.ComponentModel;

namespace FinancialTracker.ViewModels;

public class DownloadReceivingViewModel : ViewModelBase {
    private readonly SyncClient client;

    public int Progress => (int)(client.ReceivingProgress * 100);

    public DownloadReceivingViewModel(SyncClient client) {
        client.PropertyChanged += Client_PropertyChanged;
        this.client = client;
    }

    private void Client_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(SyncClient.ReceivingProgress)) {
            OnPropertyChanged(nameof(Progress));
        }
    }
}
