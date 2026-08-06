using System;
using System.Windows.Input;

namespace FinancialTracker.ViewModels;

public class UploadConnectionRequestedViewModel : UploadOpenViewModel {
    public ICommand AcceptCommand { get; }
    public ICommand RejectCommand { get; }

    public string IpAddress { get; }

    public UploadConnectionRequestedViewModel(ICommand stopServerCommand, ICommand acceptCommand, ICommand rejectCommand, string ipAddress) : base(stopServerCommand) {
        AcceptCommand = acceptCommand;
        RejectCommand = rejectCommand;
        IpAddress = ipAddress;
    }
}
