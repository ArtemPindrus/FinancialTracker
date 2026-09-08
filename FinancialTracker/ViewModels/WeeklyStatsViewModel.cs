using CommunityToolkit.Mvvm.ComponentModel;
using FinancialTracker.Domain;
using FinancialTracker.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace FinancialTracker.ViewModels;

public partial class WeeklyStatsViewModel : StatsViewModel {
    private readonly IFinancesService financesService;

    [ObservableProperty]
    public partial int SelectedYear { get; set; }

    [ObservableProperty]
    public partial int SelectedMonth { get; set; } = 1;

    public int[] AvailableYears { get; }

    public int[] AvailableMonths { get; } = Enumerable.Range(1, 12).ToArray();

    public WeeklyStatsViewModel(IFinancesService financesService) : base(5) {
        this.financesService = financesService;

        // TODO: construction
        AvailableYears = financesService.GetFinances()
                    .Select(f => f.Date.Year)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToArray();
        SelectedYear = AvailableYears[0];

        UpdateData();
    }

    protected override void GetData(out StatPoint[] total, out StatPoint[] expenses, out StatPoint[] earnings) {
        var finances = financesService.GetFinances()
            .Where(f => f.Date.Year == SelectedYear && f.Date.Month == SelectedMonth)
            .GroupBy(x => (x.Date.Day / 7) + 1);

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

    partial void OnSelectedYearChanged(int value) {
        UpdateData();
    }

    partial void OnSelectedMonthChanged(int value) {
        UpdateData();
    }
}
