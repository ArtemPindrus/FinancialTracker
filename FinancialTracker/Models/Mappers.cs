using FinancialTracket.DataAccessLayer;
using FinancialTracket.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FinancialTracker.Models {
    public static class Mappers {
        public static FinanceRecordDto ToDto(this Finance f) {
            return new FinanceRecordDto(f.Id, f.Name, f.Amount, f.Date, f.Tags.Select(t => t.Name).ToList());
        }

        public static Finance ToEntity(this FinanceRecordDto dto, AppDbContext dbContext) {
            var f = new Finance() {
                Name = dto.Name,
                Amount = dto.Amount,
                Date = dto.Date,
                Tags = dbContext.Tags
                        .Where(t => dto.Tags.Select(x => x).Contains(t.Name))
                        .ToList()
            };

            return f;
        }
    }
}
