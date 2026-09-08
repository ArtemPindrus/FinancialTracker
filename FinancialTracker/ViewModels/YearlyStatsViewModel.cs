using CommunityToolkit.Mvvm.ComponentModel;
using FinancialTracker.Domain;
using System.Linq;

namespace FinancialTracker.ViewModels {

    public partial class YearlyStatsViewModel : StatsViewModel {
        private readonly IFinancesService financesService;

        [ObservableProperty]
        public partial int SelectedYear { get; set; }

        public int[] AvailableYears { get; }

        public YearlyStatsViewModel(IFinancesService financesService) : base(12) {
            this.financesService = financesService;

            // TODO: construction should be fast
            AvailableYears = financesService.GetFinances()
                    .Select(f => f.Date.Year)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToArray();
            SelectedYear = AvailableYears[0];

            UpdateData();
        }

        partial void OnSelectedYearChanged(int value) {
            UpdateData();
        }

        protected override void GetData(out StatPoint[] total, out StatPoint[] expenses, out StatPoint[] earnings) {
            var finances = financesService.GetFinances()
                .Where(f => f.Date.Year == SelectedYear)
                .GroupBy(f => f.Date.Month);

            total = finances
                .Select(g => new StatPoint(g.Key, g.Sum(f => f.Amount)))
                .ToArray();

            expenses = finances
                .Select(g => new StatPoint(g.Key, g.Where(f => f.Amount < 0).Sum(f => f.Amount)))
                .ToArray();

            earnings = finances
                .Select(g => new StatPoint(g.Key, g.Where(f => f.Amount > 0).Sum(f => f.Amount)))
                .ToArray();
        }
    }
}
