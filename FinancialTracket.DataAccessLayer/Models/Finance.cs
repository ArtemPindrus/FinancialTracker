namespace FinancialTracket.DataAccessLayer.Models {
    public class Finance : BaseEntity {
        public string Name { get; set; }
        public double Amount { get; set; }
        public DateOnly Date { get; set; }


        public ICollection<Tag> Tags { get; set; }
    }
}
