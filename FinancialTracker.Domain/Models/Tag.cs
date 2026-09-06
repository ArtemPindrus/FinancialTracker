namespace FinancialTracker.Domain.Models {
    public class Tag : BaseEntity {
        public string Name { get; set; }

        public ICollection<Finance> Finances { get; set; }
    }
}
