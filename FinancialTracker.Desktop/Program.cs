using System;
using Avalonia;
using FinancialTracker.DataAccessLayer.Services;
using FinancialTracker.Desktop.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialTracker.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) {
        IServiceCollection desktopServices = new ServiceCollection();
        desktopServices.AddSingleton<IDatabasePathProvider, DesktopDatabasePathProvider>();

        App.ConfigureServices(desktopServices);

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp() {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}
