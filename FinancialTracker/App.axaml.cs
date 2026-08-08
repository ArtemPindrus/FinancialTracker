using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FinancialTracker.ViewModels;
using FinancialTracker.Views;
using FinancialTracket.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FinancialTracker;

public partial class App : Application
{
    private static IServiceCollection? services;

    public static void ConfigureServices(IServiceCollection serviceCollection) {
        services = serviceCollection;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
#if DEBUG
        this.AttachDeveloperTools();
#endif
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (Design.IsDesignMode) return;

        if (services is null) throw new Exception("Services were not configured.");

        services.InjectCommonServices();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        using (AppDbContext db = serviceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext()) {
            db.Database.Migrate();
        }

        MainViewModel mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new MainWindow {
                DataContext = mainViewModel
            };
        } else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime) {
            singleViewFactoryApplicationLifetime.MainViewFactory = () => {
                return new MainView { 
                    DataContext = serviceProvider.GetRequiredService<MainViewModel>() 
                };
            };
        } else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform) {
            singleViewPlatform.MainView = new MainView {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}