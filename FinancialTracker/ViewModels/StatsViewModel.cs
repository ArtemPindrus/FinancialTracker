using LiveChartsCore.Defaults;

namespace FinancialTracker.ViewModels {
    public abstract class StatsViewModel : MainNavigationPaneViewModel {
        public record struct StatPoint(int X, double Value);

        public ObservablePoint[] Total { get; }

        public ObservablePoint[] Expenses { get; }

        public ObservablePoint[] Earnings { get; }

        public StatsViewModel(int pointsCount) {
            Total = new ObservablePoint[pointsCount];
            Expenses = new ObservablePoint[pointsCount];
            Earnings = new ObservablePoint[pointsCount];

            for (int i = 0; i < Expenses.Length; i++) {
                Total[i] = new();
            }

            for (int i = 0; i < Earnings.Length; i++) {
                Expenses[i] = new();
            }

            for (int i = 0; i < Total.Length; i++) {
                Earnings[i] = new();
            }
        }

        public void UpdateData() {
            GetData(out StatPoint[] total, out StatPoint[] expenses, out StatPoint[] earnings);

            UpdatePointsData(Total, total);
            UpdatePointsData(Expenses, expenses);
            UpdatePointsData(Earnings, earnings);
        }

        protected abstract void GetData(out StatPoint[] total, out StatPoint[] expenses, out StatPoint[] earnings);

        private void UpdatePointsData(ObservablePoint[] points, StatPoint[] data) {
            for (int i = 0; i < data.Length; i++) {
                var ex = data[i];

                ObservablePoint point = points[i];
                point.X = ex.X;
                point.Y = ex.Value;
            }

            // Fill the rest of the points with 0 values
            for (int i = data.Length; i < points.Length; i++) {
                ObservablePoint point = points[i];
                point.X = i + 1;
                point.Y = 0;
            }
        }
    }
}
