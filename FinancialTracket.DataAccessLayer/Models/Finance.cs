namespace FinancialTracket.DataAccessLayer.Models {
    public class Finance : BaseEntity {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        

        public ICollection<Tag> Tags { get; set; }
    }
}
