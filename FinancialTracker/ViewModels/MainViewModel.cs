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
        if (ViewModel is not null) {
            if (!ViewModel.CheckCanSafelyClose(out string message)) {
                ICommand cancelCommand = new RelayCommand(() => {
                    DialogHostAvalonia.DialogHost.Close(null);
                });
                ICommand continueCommand = new RelayCommand(async () => {
                    DialogHostAvalonia.DialogHost.Close(null);
                    await NavigateToAsync(newValue);
                });

                MainNavigationUnsafePopupViewModel mainNavigationUnsafePopupViewModel = new(message, cancelCommand, continueCommand);

                _ = DialogHostAvalonia.DialogHost.Show(mainNavigationUnsafePopupViewModel);

                return;
            }

            if (ViewModel is IDisposable ds) ds.Dispose();
        }

        await NavigateToAsync(newValue);
    }

    async Task NavigateToAsync(NavigationViewItem? newValue) {
        if (newValue is null) {
            ViewModel = null;
            return;
        }

        MainNavigationPaneViewModel newVm = newValue.Content switch {
            "Finances" => viewModelResolver.ResolveViewModel<FinancesViewModel>(),
            "Raw Query" => viewModelResolver.ResolveViewModel<RawQueryViewModel>(),
            "Yearly Expenses" => viewModelResolver.ResolveViewModel<YearlyExpensesViewModel>(),
            "Download" => viewModelResolver.ResolveViewModel<DownloadViewModel>(),
            "Upload" => viewModelResolver.ResolveViewModel<UploadViewModel>(),
            _ => throw new NotImplementedException($"No view model implemented for navigation item with content '{newValue?.Content}'")
        };

        ViewModel = newVm;
    }
}
