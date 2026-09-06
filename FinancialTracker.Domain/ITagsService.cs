using FinancialTracker.Domain.Models;

namespace FinancialTracker.Domain {
    public interface ITagsService {
        IEnumerable<Tag> GetTags();
    }

    public class TagsService {
    }
}
