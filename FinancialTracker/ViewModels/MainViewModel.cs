using CommunityToolkit.Mvvm.ComponentModel;
using FinancialTracker.Services;
using FluentAvalonia.UI.Controls;
using System;
using System.Threading.Tasks;

namespace FinancialTracker.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private string lastNavigationItemContent = string.Empty;

    private readonly ViewModelResolver viewModelResolver;

    [ObservableProperty]
    public partial FANavigationViewItem? SelectedNavigationItem { get; set; }

    [ObservableProperty]
    public partial MainNavigationPaneViewModel? ViewModel { get; set; }

    public FAInfoBar InfoBar { get; }

    public MainViewModel(ViewModelResolver viewModelResolver) {
        this.viewModelResolver = viewModelResolver;

        InfoBar = new() {
            IsClosable = true,
            IsOpen = false,
        };
    }

    async partial void OnSelectedNavigationItemChanged(FANavigationViewItem? oldValue, FANavigationViewItem? newValue) {
        await NavigateToSafeAsync(oldValue, newValue);
    }

    async Task NavigateToSafeAsync(FANavigationViewItem? oldValue, FANavigationViewItem? newValue) {
        if (newValue?.Content is not string newNavigationString
            || newNavigationString == lastNavigationItemContent) return;

        if (ViewModel is not null) {
            if (!ViewModel.CheckCanSafelyClose(out string message)) {
                _ = MainNavigationUnsafePopupViewModel.ShowInPopup(message,
                    DialogHostHelper.MainDialogIdentifier,
                    cancelAction: () => SelectedNavigationItem = oldValue,
                    continueAction: async () => await NavigateToAsync(newNavigationString));

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
