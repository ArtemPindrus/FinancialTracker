using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using FinancialTracker.Android.Services;
using FinancialTracker.DataAccessLayer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialTracker.Android;

[Activity(
    Label = "FinancialTracker.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder) {
        IServiceCollection androidServices = new ServiceCollection();
        androidServices.AddSingleton<IDatabasePathProvider, AndroidDatabasePathProvider>();

        App.ConfigureServices(androidServices);

        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}
