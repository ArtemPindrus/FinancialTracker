using FinancialTracker.Models;
using FinancialTracket.DataAccessLayer;
using FinancialTracket.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialTracker {
    public static class DbHelper {
        public static Task SaveModificationsAsync(this AppDbContext dbContext, IEnumerable<FinanceRecordDto> finances) {
            return Task.Run(async () => {
                dbContext.ApplyModifications(finances);
                dbContext.SaveChanges();
            });
        }

        public static void ApplyModifications(this AppDbContext dbContext, IEnumerable<FinanceRecordDto> finances) {
            var modified = finances.Where(x => x.IsModified);
            var added = finances.Where(x => x.IsAdded);
            var deleted = finances.Where(x => x.IsDeleted);

            foreach (var d in deleted) {
                var f = dbContext.Finances.Where(x => x.Id == d.Id).Single();
                dbContext.Finances.Remove(f);
            }

            foreach (FinanceRecordDto m in modified) {
                Finance f = dbContext.Finances
                    .Where(x => x.Id == m.Id)
                    .Include(x => x.Tags)
                    .Single();

                dbContext.AddMissingTagsToDatabase(m);
                dbContext.SaveChanges();

                ApplyDtoToEntity(m, f, dbContext);
            }

            foreach (FinanceRecordDto a in added) {
                dbContext.AddMissingTagsToDatabase(a);
                dbContext.SaveChanges();

                Finance f = a.ToEntity(dbContext);
                dbContext.Finances.Add(f);
            }
        }

        public static void ApplyDtoToEntity(FinanceRecordDto dto, Finance entity, AppDbContext dbContext) {
            entity.Name = dto.Name;
            entity.Amount = dto.Amount;
            entity.Date = dto.Date;
            entity.Tags = dbContext.Tags
                .Where(t => dto.Tags.Select(x => x).Contains(t.Name))
                .ToList();
        }

        public static void AddMissingTagsToDatabase(this AppDbContext dbContext, FinanceRecordDto fr) {
            var existingTagsNames = dbContext.Tags
                .Select(t => t.Name.ToLower())
                .ToList();

            var recordTags = fr.Tags;

            IEnumerable<Tag> absentTags = recordTags
                .Where(x => !string.IsNullOrEmpty(x) && !existingTagsNames.Contains(x.ToLower()))
                .Distinct()
                .Select(x => new Tag() { Name = x });

            dbContext.Tags.AddRange(absentTags);
        }
    }
}
