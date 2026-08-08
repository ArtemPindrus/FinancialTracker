using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using FinancialTracker.Android.Services;
using FinancialTracker.DataAccessLayer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialTracker.Android {
    [Application]
    public class Application : AvaloniaAndroidApplication<App> {
        protected Application(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) {
        }

        protected override AppBuilder CustomizeAppBuilder(AppBuilder builder) {
            IServiceCollection androidServices = new ServiceCollection();
            androidServices.AddSingleton<IDatabasePathProvider, AndroidDatabasePathProvider>();

            App.ConfigureServices(androidServices);

            return base.CustomizeAppBuilder(builder)
                .WithInterFont();
        }
    }
}
