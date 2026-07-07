using CommunityToolkit.Mvvm.ComponentModel;
using FinancialTracker.Services;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;

namespace FinancialTracker.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private IViewCreator<FinancesViewModel> financesViewModelCreator;
    private IViewCreator<RawQueryViewModel> rawQueryViewModelCreator;
    private IViewCreator<YearlyExpensesViewModel> yearlyExpensesViewModelCreator;
    private readonly IViewCreator<DownloadViewModel> downloadViewModelCreator;
    private readonly IViewCreator<UploadViewModel> uploadViewModelCreator;

    [ObservableProperty]
    private NavigationViewItem? selectedNavigationItem;

    [ObservableProperty]
    private ViewModelBase? viewModel;

    public List<string?> Items { get; } = [null, "1", "2", "3"];

    public List<string> Selected { get; } = ["1", "2"];

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

    partial void OnSelectedNavigationItemChanged(NavigationViewItem? oldValue, NavigationViewItem? newValue) {
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
    }
}
