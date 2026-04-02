namespace FinancialTracker.ViewModels {
    public class AffectedRowsQueryResultViewModel : ViewModelBase {
        public int Rows { get; }

        public AffectedRowsQueryResultViewModel(int rows) {
            Rows = rows;
        }
    }
}
