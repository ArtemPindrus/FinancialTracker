using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FinancialTracket.DataAccessLayer {
    public class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext> {
        public AppDbContext CreateDbContext(string[] args) {
            DbContextOptions<AppDbContext> opt = new DbContextOptionsBuilder<AppDbContext>().UseSqlite().Options;
            AppDbContext c = new(opt);

            return c;
        }
    }
}
