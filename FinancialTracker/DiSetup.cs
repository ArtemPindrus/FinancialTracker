using FinancialTracker.Domain;
using FinancialTracker.Services;
using FinancialTracker.StateMachines;
using FinancialTracker.ViewModels;
using FinancialTracket.DataAccessLayer;
using FinancialTracket.DataAccessLayer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialTracker {
    public static class DiSetup {
        public static IServiceCollection InjectCommonServices(this IServiceCollection services) {
            services.SetupDataAccessLayer();

            services.AddSingleton<MainViewModel>();

            services.AddTransient<FinancesViewModel>();
            services.AddTransient<RawQueryViewModel>();
            services.AddTransient<YearlyExpensesViewModel>();
            services.AddTransient<DownloadViewModel>();
            services.AddTransient<UploadViewModel>();


            services.AddSingleton<ViewModelResolver>();
            services.AddSingleton<IErrorNotifier, InfoBarNotifier>((sp) => {
                MainViewModel mainViewModel = sp.GetRequiredService<MainViewModel>();

                return new(mainViewModel.InfoBar);
            });

            services.AddTransient<SyncClient>();
            services.AddTransient<SyncServer>();



            services.AddTransient<IRawSqlService, RawSqlService>();
            services.AddTransient<IFinancesService, SqlFinancesService>();
            services.AddTransient<ITagsService, SqlTagsService>();


            return services;
        }
    }
}
