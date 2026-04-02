using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using FinancialTracker.Services;
using FinancialTracker.ViewModels;
using FinancialTracker.Views;
using FinancialTracket.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialTracker;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Non-UI thread exceptions
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            var ex = e.ExceptionObject as Exception;
            Console.WriteLine($"AVALONIA: AppDomain unhandled: {ex?.Message}");
        };

        // Unobserved Task exceptions
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            Console.WriteLine($"AVALONIA: Task unobserved: {e.Exception.Message}");
            e.SetObserved();
        };

        IServiceCollection services = new ServiceCollection()
            .InjectCommonServices();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        AppDbContext db = serviceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();

        MainViewModel mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}