using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinancialTracker.ViewModels;

public class MainNavigationUnsafePopupViewModel : ViewModelBase {
    public string Message { get; }
    public ICommand CancelCommand { get; }
    public ICommand ContinueCommand { get; }

    public MainNavigationUnsafePopupViewModel(string message, ICommand cancelCommand, ICommand continueCommand) {
        Message = message;
        CancelCommand = cancelCommand;
        ContinueCommand = continueCommand;
    }

    public static Task ShowInPopup(string message,
        string dialogHostIdentifier,
        Action cancelAction,
        Action continueAction) {
        ICommand cancelCommand = new RelayCommand(() => {
            cancelAction();

            DialogHostAvalonia.DialogHost.Close(dialogHostIdentifier);
        });

        ICommand continueCommand = new RelayCommand(() => {
            continueAction();

            DialogHostAvalonia.DialogHost.Close(dialogHostIdentifier);
        });

        MainNavigationUnsafePopupViewModel vm = new(message, cancelCommand, continueCommand);
        return DialogHostAvalonia.DialogHost.Show(vm, dialogHostIdentifier);
    }
}
