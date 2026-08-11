using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
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

        if (DataContext is not FinancesViewModel vm) throw new Exception("Unexpected DataContext type.");

        INavigationService navigationService = new NavigationPageNavigationServicer(NavigationPage);

        vm.NavigationService = navigationService;
    }
}