using CommunityToolkit.Mvvm.ComponentModel;
using FinancialTracker.Services;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;

namespace FinancialTracker.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly ViewModelResolver viewModelResolver;

    [ObservableProperty]
    public partial NavigationViewItem? SelectedNavigationItem { get; set; }

    [ObservableProperty]
    public partial ViewModelBase? ViewModel { get; set; }

    public MainViewModel(ViewModelResolver viewModelResolver) {
        this.viewModelResolver = viewModelResolver;
    }

    async partial void OnSelectedNavigationItemChanged(NavigationViewItem? oldValue, NavigationViewItem? newValue) {
        if (ViewModel is IDisposable ds) ds.Dispose();

        if (newValue is null) {
            ViewModel = null;
            return;
        }

        ViewModelBase vm = newValue.Content switch {
            "Finances" => viewModelResolver.ResolveViewModel<FinancesViewModel>(),
            "Raw Query" => viewModelResolver.ResolveViewModel<RawQueryViewModel>(),
            "Yearly Expenses" => viewModelResolver.ResolveViewModel<YearlyExpensesViewModel>(),
            "Download" => viewModelResolver.ResolveViewModel<DownloadViewModel>(),
            "Upload" => viewModelResolver.ResolveViewModel<UploadViewModel>(),
            _ => throw new NotImplementedException($"No view model implemented for navigation item with content '{newValue?.Content}'")
        };

        ViewModel = vm;

        if (vm is FinancesViewModel fvm) {
            await fvm.PopulateTableAsync();
        }
    }
}
