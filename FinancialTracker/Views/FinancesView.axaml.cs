using Avalonia.Controls;
using FinancialTracker.Services;
using FinancialTracker.ViewModels;
using System;

namespace FinancialTracker.Views;

public partial class FinancesView : UserControl
{
    public FinancesView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e) {
        base.OnDataContextChanged(e);

        if (DataContext is not FinancesViewModel vm) return;

        INavigationService navigationService = new NavigationPageNavigationServicer(NavigationPage);

        vm.NavigationService = navigationService;
    }
}