using System.Windows.Input;

namespace FinancialTracker.ViewModels {
    public class UploadDisconnectedViewModel : ViewModelBase {
        public ICommand TryConnectingCommand { get; }

        public UploadDisconnectedViewModel(ICommand tryConnectingCommand) {
            TryConnectingCommand = tryConnectingCommand;
        }
    }
}
