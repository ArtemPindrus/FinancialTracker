using FinancialTracker.Domain;
using FinancialTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancialTracket.DataAccessLayer.Services {
    public class SqlFinancesService : IFinancesService {
        private readonly IDbContextFactory<AppDbContext> dbContextFactory;

        public SqlFinancesService(IDbContextFactory<AppDbContext> dbContextFactory) {
            this.dbContextFactory = dbContextFactory;
        }

        public void AddFinances(IEnumerable<Finance> finances) {
            using var context = dbContextFactory.CreateDbContext();

            context.Finances.AddRange(finances);
            context.SaveChanges();
        }

        public void DeleteFinances(IEnumerable<int> finances) {
            using var context = dbContextFactory.CreateDbContext();

            var financesToDelete = context.Finances.Where(f => finances.Contains(f.Id));
            context.Finances.RemoveRange(financesToDelete);

            context.SaveChanges();
        }

        public IEnumerable<Finance> GetFinances() {
            using var context = dbContextFactory.CreateDbContext();

            return context.Finances
                .AsNoTracking()
                .Include(f => f.Tags)
                .ToList();
        }

        public void UpdateFinances(IEnumerable<Finance> finances) {
            using var context = dbContextFactory.CreateDbContext();

            foreach (var newFinance in finances) {
                Finance? origFinance = context.Finances
                    .Include(f => f.Tags)
                    .FirstOrDefault(finance => finance.Id == newFinance.Id);

                if (origFinance != null) {
                    origFinance.Name = newFinance.Name;
                    origFinance.Amount = newFinance.Amount;
                    origFinance.Date = newFinance.Date;

                    origFinance.Tags.Clear();
                    foreach (var newTag in newFinance.Tags) {
                        var origTag = context.Tags
                            .FirstOrDefault(tag => tag.Id == newTag.Id);

                        if (origTag is not null) origFinance.Tags.Add(origTag);
                    }
                }
            }

            context.SaveChanges();
        }
    }
}
