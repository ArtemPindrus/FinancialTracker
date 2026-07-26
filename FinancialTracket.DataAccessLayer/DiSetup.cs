using FinancialTracker.DataAccessLayer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialTracket.DataAccessLayer {
    public static class DiSetup {
        public static IServiceCollection SetupDataAccessLayer(this IServiceCollection services) {
            services.AddDbContextFactory<AppDbContext>((sp, o) => {
                string databasePath = sp.GetRequiredService<IDatabasePathProvider>().GetDatabasePath();
                string connectionString = $"Data Source={databasePath};Pooling=False";

                o.UseSqlite(connectionString);
            });

            return services;
        }
    }
}
