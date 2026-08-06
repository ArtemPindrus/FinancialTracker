using System.Windows.Input;

namespace FinancialTracker.ViewModels {
    public class UploadOpenViewModel : ViewModelBase {
        public ICommand StopServerCommand { get; }

        public UploadOpenViewModel(ICommand stopServerCommand) {
            StopServerCommand = stopServerCommand;
        }
    }
}
