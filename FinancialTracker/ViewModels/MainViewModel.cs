using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialTracker.Services;
using FluentAvalonia.UI.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinancialTracker.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private string lastNavigationItemContent = string.Empty;

    private readonly ViewModelResolver viewModelResolver;

    [ObservableProperty]
    public partial NavigationViewItem? SelectedNavigationItem { get; set; }

    [ObservableProperty]
    public partial MainNavigationPaneViewModel? ViewModel { get; set; }

    public MainViewModel(ViewModelResolver viewModelResolver) {
        this.viewModelResolver = viewModelResolver;
    }

    async partial void OnSelectedNavigationItemChanged(NavigationViewItem? oldValue, NavigationViewItem? newValue) {
        await NavigateToSafeAsync(oldValue, newValue);
    }

    async Task NavigateToSafeAsync(NavigationViewItem? oldValue, NavigationViewItem? newValue) {
        if (newValue?.Content is not string newNavigationString
            || newNavigationString == lastNavigationItemContent) return;

        if (ViewModel is not null) {
            if (!ViewModel.CheckCanSafelyClose(out string message)) {
                ICommand cancelCommand = new RelayCommand(() => {
                    DialogHostAvalonia.DialogHost.Close(null);
                    SelectedNavigationItem = oldValue;
                });
                ICommand continueCommand = new RelayCommand(async () => {
                    DialogHostAvalonia.DialogHost.Close(null);
                    await NavigateToAsync(newNavigationString);
                });

                MainNavigationUnsafePopupViewModel mainNavigationUnsafePopupViewModel = new(message, cancelCommand, continueCommand);

                _ = DialogHostAvalonia.DialogHost.Show(mainNavigationUnsafePopupViewModel);

                return;
            }

            if (ViewModel is IDisposable ds) ds.Dispose();
        }

        await NavigateToAsync(newNavigationString);
    }

    async Task NavigateToAsync(string newNavigationString) {
        MainNavigationPaneViewModel newVm = newNavigationString switch {
            "Finances" => viewModelResolver.ResolveViewModel<FinancesViewModel>(),
            "Raw Query" => viewModelResolver.ResolveViewModel<RawQueryViewModel>(),
            "Yearly Expenses" => viewModelResolver.ResolveViewModel<YearlyExpensesViewModel>(),
            "Download" => viewModelResolver.ResolveViewModel<DownloadViewModel>(),
            "Upload" => viewModelResolver.ResolveViewModel<UploadViewModel>(),
            _ => throw new NotImplementedException($"No view model implemented for navigation item with content '{newNavigationString}'")
        };

        ViewModel = newVm;
        lastNavigationItemContent = newNavigationString;
        await ViewModel.Initialize();
    }
}
