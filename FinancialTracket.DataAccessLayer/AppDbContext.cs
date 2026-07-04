using FinancialTracket.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancialTracket.DataAccessLayer {
    public class AppDbContext : DbContext {
        public DbSet<Finance> Finances { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Tag>().ToTable(
                t => {
                    t.HasCheckConstraint("CK_Tag_Name_NonWhitespace", "length(trim(\"Name\")) > 0");
                }
            );
        }
    }
}
