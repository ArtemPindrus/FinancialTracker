using FinancialTracker.Domain;
using FinancialTracker.Domain.Models;
using System.Linq;

namespace FinancialTracker.Models {
    public static class Mappers {
        public static FinanceRecordDto ToDto(this Finance f) {
            return new FinanceRecordDto(f.Id, f.Name, f.Amount, f.Date, f.Tags.Select(t => t.Name).ToList());
        }

        public static Finance ToEntity(this FinanceRecordDto dto, ITagsService tagsService) {
            var f = new Finance() {
                Id = dto.Id,
                Name = dto.Name,
                Amount = dto.Amount,
                Date = dto.Date,
                Tags = tagsService.GetTags()
                        .Where(t => dto.Tags.Contains(t.Name))
                        .ToArray()
            };

            if (dto.IsAdded) f.Id = 0; // Reset Id for new records

            return f;
        }
    }
}
