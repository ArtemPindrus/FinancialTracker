using FinancialTracker.Services;
using FinancialTracker.ViewModels;
using FinancialTracket.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialTracker {
    public static class DiSetup {
        public static IServiceCollection InjectCommonServices(this IServiceCollection services, IConfiguration config) {
            string connectionString = $"Data Source={config.GetDatabasePath()}";

            services.AddDbContext<AppDbContext>(x => x.UseSqlite(connectionString));

            services.AddTransient<FinancesViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<RawQueryViewModel>();

            services.AddSingleton<IViewCreator<FinancesViewModel>, ViewCreator<FinancesViewModel>>();
            services.AddSingleton<IViewCreator<RawQueryViewModel>, ViewCreator<RawQueryViewModel>>();
            services.AddSingleton(config);

            return services;
        }
    }
}
