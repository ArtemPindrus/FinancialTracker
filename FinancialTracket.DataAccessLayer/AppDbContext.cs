using FinancialTracket.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancialTracket.DataAccessLayer {
    public class AppDbContext : DbContext {
        public DbSet<Finance> Finances { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        }
    }
}
