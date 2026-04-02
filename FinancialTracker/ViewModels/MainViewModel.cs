using CommunityToolkit.Mvvm.ComponentModel;
using FinancialTracker.Services;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;

namespace FinancialTracker.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private IViewCreator<FinancesViewModel> financesViewModelCreator;

    [ObservableProperty]
    private NavigationViewItem? selectedNavigationItem;

    [ObservableProperty]
    private ViewModelBase? viewModel;

    public List<string?> Items { get; } = [null, "1", "2", "3"];

    public List<string> Selected { get; } = ["1", "2"];

    public MainViewModel(IViewCreator<FinancesViewModel> financesViewModelCreator) {
        this.financesViewModelCreator = financesViewModelCreator;
    }

    partial void OnSelectedNavigationItemChanged(NavigationViewItem? value) {
        ViewModelBase vm = value switch {
            { Content: "Finances" } => financesViewModelCreator.Create(),
            _ => throw new NotImplementedException($"No view model implemented for navigation item with content '{value?.Content}'")
        };

        ViewModel = vm;
    }
}
