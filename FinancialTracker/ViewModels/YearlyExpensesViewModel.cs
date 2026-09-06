using CommunityToolkit.Mvvm.ComponentModel;
using FinancialTracker.Domain;
using FinancialTracker.Domain.Models;
using LiveChartsCore.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialTracker.ViewModels {
    public partial class YearlyExpensesViewModel : MainNavigationPaneViewModel {
        private readonly IFinancesService financesService;

        [ObservableProperty]
        public int selectedYear;

        public ObservablePoint[] Total { get; } = new ObservablePoint[12];

        public ObservablePoint[] Expenses { get; } = new ObservablePoint[12];

        public ObservablePoint[] Earnings { get; } = new ObservablePoint[12];

        public int[] AvailableYears { get; }

        public YearlyExpensesViewModel(IFinancesService financesService) {
            this.financesService = financesService;

            for (int i = 0; i < Expenses.Length; i++) {
                Expenses[i] = new();
            }

            for (int i = 0; i < Earnings.Length; i++) {
                Earnings[i] = new();
            }

            for (int i = 0; i < Total.Length; i++) {
                Total[i] = new();
            }

            AvailableYears = financesService.GetFinances()
                    .Select(f => f.Date.Year)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToArray();

            selectedYear = AvailableYears[0];

            // TODO: construction should be fast
            UpdateData();
        }

        partial void OnSelectedYearChanged(int value) {
            UpdateData();
        }

        public async Task UpdateDataAsync() {
            await Task.Run(UpdateData);
        }

        public void UpdateData() {
            IEnumerable<Finance> finances = financesService.GetFinances();

            // expenses
            var expenses = finances
                .Where(f => f.Amount < 0)
                .Where(f => f.Date.Year == SelectedYear)
                .GroupBy(f => f.Date.Month)
                .Select(g => new ValueTuple<int, double>(g.Key, (double)g.Sum(f => f.Amount)))
                .ToArray();

            UpdatePointsData(Expenses, expenses);

            // earnings
            var earnings = finances
                .Where(f => f.Amount > 0)
                .Where(f => f.Date.Year == SelectedYear)
                .GroupBy(f => f.Date.Month)
                .Select(g => new ValueTuple<int, double>(g.Key, (double)g.Sum(f => f.Amount)))
                .ToArray();

            UpdatePointsData(Earnings, earnings);

            // total
            var total = finances
                .Where(f => f.Date.Year == SelectedYear)
                .GroupBy(f => f.Date.Month)
                .Select(g => new ValueTuple<int, double>(g.Key, (double)g.Sum(f => f.Amount)))
                .ToArray();

            UpdatePointsData(Total, total);
        }

        private void UpdatePointsData(ObservablePoint[] points, (int, double)[] data) {
            for (int i = 0; i < data.Length; i++) {
                var ex = data[i];

                ObservablePoint point = points[i];
                point.X = ex.Item1;
                point.Y = ex.Item2;
            }

            for (int i = data.Length; i < 12; i++) {
                ObservablePoint point = points[i];
                point.X = i + 1;
                point.Y = 0;
            }
        }
    }
}
