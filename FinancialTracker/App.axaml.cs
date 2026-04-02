using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using FinancialTracker.ViewModels;
using FinancialTracker.Views;
using Microsoft.Extensions.DependencyInjection;
using FinancialTracket.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using FinancialTracker.Services;
using Microsoft.Extensions.Configuration;
using System;
using FinancialTracket.DataAccessLayer.Models;

namespace FinancialTracker;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        IConfigurationBuilder cb = new ConfigurationBuilder();

#if DEBUG
        cb.AddJsonFile("appsettings.Development.json");
#else
        cb.AddJsonFile("appsettings.json");
#endif

        IConfiguration configuration = cb.Build();

        string connectionString = configuration.GetConnectionString("DefaultConnection");

        IServiceCollection services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(x => x.UseSqlite(connectionString));

        services.AddTransient<FinancesViewModel>();
        services.AddTransient<MainViewModel>();

        services.AddSingleton<IViewCreator<FinancesViewModel>, ViewCreator<FinancesViewModel>>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        AppDbContext db = serviceProvider.GetRequiredService<AppDbContext>();

#if DEBUG
        //db.Database.EnsureDeleted();
        //db.Database.EnsureCreated();

        //Tag groceries = new() { Name = "Groceries" };
        //Tag gaming = new() { Name = "Gaming" };

        //Finance f = new()
        //{
        //    Name = "Grocery shopping",
        //    Amount = 150.75m,
        //    Date = DateOnly.FromDateTime(DateTime.Now),
        //    Tags = [groceries]
        //};

        //Finance g = new()
        //{
        //    Name = "Zeno Clash",
        //    Amount = 59.99m,
        //    Date = DateOnly.FromDateTime(DateTime.Now),
        //    Tags = [gaming]
        //};

        //db.Add(f);
        //db.Add(g);
        //db.SaveChanges();
#endif

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