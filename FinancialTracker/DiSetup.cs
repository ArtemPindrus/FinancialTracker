using FinancialTracker.Services;
using FinancialTracker.StateMachines;
using FinancialTracker.ViewModels;
using FinancialTracket.DataAccessLayer;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialTracker {
    public static class DiSetup {
        public static IServiceCollection InjectCommonServices(this IServiceCollection services) {
            services.SetupDataAccessLayer();

            services.AddTransient<FinancesViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<RawQueryViewModel>();
            services.AddTransient<YearlyExpensesViewModel>();
            services.AddTransient<DownloadViewModel>();
            services.AddTransient<UploadViewModel>();

            services.AddSingleton<IViewCreator<FinancesViewModel>, ViewCreator<FinancesViewModel>>();
            services.AddSingleton<IViewCreator<RawQueryViewModel>, ViewCreator<RawQueryViewModel>>();
            services.AddSingleton<IViewCreator<YearlyExpensesViewModel>, ViewCreator<YearlyExpensesViewModel>>();
            services.AddSingleton<IViewCreator<DownloadViewModel>, ViewCreator<DownloadViewModel>>();
            services.AddSingleton<IViewCreator<UploadViewModel>, ViewCreator<UploadViewModel>>();

            services.AddTransient<SyncClient>();
            services.AddTransient<SyncServer>();

            return services;
        }
    }
}
