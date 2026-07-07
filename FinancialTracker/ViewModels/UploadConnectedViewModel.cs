using System.Windows.Input;

namespace FinancialTracker.ViewModels {
    public class UploadConnectedViewModel : ViewModelBase {
        public string ClientId { get; }

        public ICommand DisconnectCommand { get; }
        public ICommand SendCommand { get; }

        public UploadConnectedViewModel(string clientId, ICommand disconnectCommand, ICommand sendCommand) {
            ClientId = clientId;
            DisconnectCommand = disconnectCommand;
            SendCommand = sendCommand;
        }
    }
}
