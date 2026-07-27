using CommunityToolkit.Mvvm.ComponentModel;
using FinancialTracker.Services;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;

namespace FinancialTracker.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IViewCreator<FinancesViewModel> financesViewModelCreator;
    private readonly IViewCreator<RawQueryViewModel> rawQueryViewModelCreator;
    private readonly IViewCreator<YearlyExpensesViewModel> yearlyExpensesViewModelCreator;
    private readonly IViewCreator<DownloadViewModel> downloadViewModelCreator;
    private readonly IViewCreator<UploadViewModel> uploadViewModelCreator;

    [ObservableProperty]
    public partial NavigationViewItem? SelectedNavigationItem { get; set; }

    [ObservableProperty]
    public partial ViewModelBase? ViewModel { get; set; }

    public MainViewModel(IViewCreator<FinancesViewModel> financesViewModelCreator,
        IViewCreator<RawQueryViewModel> rawQueryViewModelCreator,
        IViewCreator<YearlyExpensesViewModel> yearlyExpensesViewModelCreator,
        IViewCreator<DownloadViewModel> downloadViewModelCreator,
        IViewCreator<UploadViewModel> uploadViewModelCreator) {
        this.financesViewModelCreator = financesViewModelCreator;
        this.rawQueryViewModelCreator = rawQueryViewModelCreator;
        this.yearlyExpensesViewModelCreator = yearlyExpensesViewModelCreator;
        this.downloadViewModelCreator = downloadViewModelCreator;
        this.uploadViewModelCreator = uploadViewModelCreator;
    }

    async partial void OnSelectedNavigationItemChanged(NavigationViewItem? oldValue, NavigationViewItem? newValue) {
        if (ViewModel is IDisposable ds) ds.Dispose();

        if (newValue is null) {
            ViewModel = null;
            return;
        }

        ViewModelBase vm = newValue.Content switch {
            "Finances" => financesViewModelCreator.Create(),
            "Raw Query" => rawQueryViewModelCreator.Create(),
            "Yearly Expenses" => yearlyExpensesViewModelCreator.Create(),
            "Download" => downloadViewModelCreator.Create(),
            "Upload" => uploadViewModelCreator.Create(),
            _ => throw new NotImplementedException($"No view model implemented for navigation item with content '{newValue?.Content}'")
        };

        ViewModel = vm;

        if (vm is FinancesViewModel fvm) {
            await fvm.PopulateTableAsync();
        }
    }
}
