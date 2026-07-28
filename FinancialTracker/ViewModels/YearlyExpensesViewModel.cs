using CommunityToolkit.Mvvm.ComponentModel;
using FinancialTracket.DataAccessLayer;
using LiveChartsCore.Defaults;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System;
using System.Linq;

namespace FinancialTracker.ViewModels {
    public partial class YearlyExpensesViewModel : ViewModelBase {
        private readonly IDbContextFactory<AppDbContext> dbContextFactory;

        [ObservableProperty]
        public int selectedYear;

        public ObservablePoint[] Total { get; } = new ObservablePoint[12];

        public ObservablePoint[] Expenses { get; } = new ObservablePoint[12];

        public ObservablePoint[] Earnings { get; } = new ObservablePoint[12];

        public int[] AvailableYears { get; }

        public YearlyExpensesViewModel(IDbContextFactory<AppDbContext> dbContextFactory) {
            this.dbContextFactory = dbContextFactory;

            for (int i = 0; i < Expenses.Length; i++) {
                Expenses[i] = new();
            }

            for (int i = 0; i < Earnings.Length; i++) {
                Earnings[i] = new();
            }

            for (int i = 0; i < Total.Length; i++) {
                Total[i] = new();
            }

            using (var dbContext = dbContextFactory.CreateDbContext()) {
                AvailableYears = dbContext.Finances
                    .Select(f => f.Date.Year)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToArray();
            }

            selectedYear = AvailableYears[0];

            UpdateData();
        }

        partial void OnSelectedYearChanged(int value) {
            UpdateData();
        }

        public void UpdateData() {
            using var dbContext = dbContextFactory.CreateDbContext();

            // expenses
            var expenses = dbContext.Finances
                .Where(f => f.Amount < 0)
                .Where(f => f.Date.Year == SelectedYear)
                .GroupBy(f => f.Date.Month)
                .Select(g => new ValueTuple<int, double>(g.Key, (double)g.Sum(f => f.Amount)))
                .ToArray();

            UpdatePointsData(Expenses, expenses);

            // earnings
            var earnings = dbContext.Finances
                .Where(f => f.Amount > 0)
                .Where(f => f.Date.Year == SelectedYear)
                .GroupBy(f => f.Date.Month)
                .Select(g => new ValueTuple<int, double>(g.Key, (double)g.Sum(f => f.Amount)))
                .ToArray();

            UpdatePointsData(Earnings, earnings);

            // total
            var total = dbContext.Finances
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
