using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FinancialTracket.DataAccessLayer {
    public class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext> {
        public AppDbContext CreateDbContext(string[] args) {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build();

            DbContextOptions<AppDbContext> opt = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(configuration.GetConnectionString("DefaultConnection"))
                .Options;

            AppDbContext c = new(opt);

            return c;
        }
    }
}
