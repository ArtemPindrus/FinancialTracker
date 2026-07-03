using FinancialTracker.Models;
using FinancialTracket.DataAccessLayer;
using FinancialTracket.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialTracker {
    public static class DbHelper {
        public static void SyncDtoToEntity(FinanceRecordDto dto, Finance entity, AppDbContext dbContext) {
            entity.Name = dto.Name;
            entity.Amount = dto.Amount;
            entity.Date = dto.Date;
            entity.Tags = dbContext.Tags
                .Where(t => dto.Tags.Select(x => x).Contains(t.Name))
                .ToList();
        }

        public static async Task AddMissingTagsToDatabaseAsync(this AppDbContext dbContext, FinanceRecordDto fr) {
            var existingTagsNames = dbContext.Tags
                .Select(t => t.Name.ToLower())
                .ToList();

            var recordTags = fr.Tags;

            IEnumerable<Tag> absentTags = recordTags
                .Where(x => !existingTagsNames.Contains(x.ToLower()))
                .Distinct()
                .Select(x => new Tag() { Name = x });

            await dbContext.Tags.AddRangeAsync(absentTags);
        }
    }
}
