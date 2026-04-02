using FinancialTracker.Services;
using FinancialTracker.ViewModels;
using FinancialTracket.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace FinancialTracker {
    public static class DiSetup {
        public static IServiceCollection InjectCommonServices(this IServiceCollection services) {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "finances.db");
            string connectionString = $"Data Source={dbPath}";

            services.AddDbContext<AppDbContext>(x => x.UseSqlite(connectionString));

            services.AddTransient<FinancesViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<RawQueryViewModel>();

            services.AddSingleton<IViewCreator<FinancesViewModel>, ViewCreator<FinancesViewModel>>();
            services.AddSingleton<IViewCreator<RawQueryViewModel>, ViewCreator<RawQueryViewModel>>();

            return services;
        }
    }
}
