using System.Windows.Input;

namespace FinancialTracker.ViewModels {
    public class UploadConnectingViewModel : ViewModelBase {
        public ICommand CancelConnectionCommand { get; }

        public UploadConnectingViewModel(ICommand cancelConnection) {
            CancelConnectionCommand = cancelConnection;
        }
    }
}
