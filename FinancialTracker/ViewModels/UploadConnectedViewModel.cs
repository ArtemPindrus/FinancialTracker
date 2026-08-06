using System.Windows.Input;

namespace FinancialTracker.ViewModels {
    public class UploadConnectedViewModel : UploadOpenViewModel {
        public ICommand DisconnectCommand { get; }
        public string IpAddress { get; }

        public UploadConnectedViewModel(ICommand stopServerCommand, ICommand disconnectCommand, string ipAddress) : base(stopServerCommand) {
            DisconnectCommand = disconnectCommand;
            IpAddress = ipAddress;
        }
    }
}
