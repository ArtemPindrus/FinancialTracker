using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using FinancialTracket.DataAccessLayer;
using FinancialTracket.DataAccessLayer.Models;
using LiveChartsCore.Defaults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FinancialTracker.ViewModels {
    public partial class YearlyExpensesViewModel : ViewModelBase {
        private readonly AppDbContext dbContext;

        [ObservableProperty]
        public int selectedYear;

        public ObservablePoint[] Total { get; } = new ObservablePoint[12];

        public ObservablePoint[] Expenses { get; } = new ObservablePoint[12];

        public ObservablePoint[] Earnings { get; } = new ObservablePoint[12];

        public int[] AvailableYears { get; }

        public YearlyExpensesViewModel(AppDbContext dbContext) {
            this.dbContext = dbContext;

            for (int i = 0; i < Expenses.Length; i++) {
                Expenses[i] = new();
            }

            for (int i = 0; i < Earnings.Length; i++) {
                Earnings[i] = new();
            }

            for (int i = 0; i < Total.Length; i++) {
                Total[i] = new();
            }

            AvailableYears = dbContext.Finances
                .Select(f => f.Date.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToArray();

            selectedYear = AvailableYears[0];

            UpdateData();
        }

        partial void OnSelectedYearChanged(int value) {
            UpdateData();
        }

        public void UpdateData() {
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
