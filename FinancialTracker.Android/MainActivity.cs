using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;

namespace FinancialTracker.Android;

[Activity(
    Label = "FinancialTracker.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        AppConfig.DefaultConfigurationBuilder.UseCommonConfiguration();

        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}
