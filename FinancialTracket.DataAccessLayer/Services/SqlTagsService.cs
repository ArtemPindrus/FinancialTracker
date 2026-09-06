using FinancialTracker.Domain;
using FinancialTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancialTracket.DataAccessLayer.Services {
    public class SqlTagsService : ITagsService {
        private readonly IDbContextFactory<AppDbContext> dbContextFactory;

        public SqlTagsService(IDbContextFactory<AppDbContext> dbContextFactory) {
            this.dbContextFactory = dbContextFactory;
        }

        public IEnumerable<Tag> GetTags() {
            using var context = dbContextFactory.CreateDbContext();

            return context.Tags
                .AsNoTracking()
                .ToList();
        }
    }
}
