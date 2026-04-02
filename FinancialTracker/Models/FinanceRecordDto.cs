namespace FinancialTracker.Models {
    public record class FinanceRecordDto(int id, string name, decimal amount, string[] tags);
}
