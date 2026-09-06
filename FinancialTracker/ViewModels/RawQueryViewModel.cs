using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialTracker.Domain;
using System;
using System.Threading.Tasks;

namespace FinancialTracker.ViewModels {
    public partial class RawQueryViewModel : MainNavigationPaneViewModel {
        private readonly IRawSqlService rawSqlService;

        [ObservableProperty]
        private ViewModelBase? resultViewModel;

        public RawQueryViewModel(IRawSqlService rawSqlService) {
            this.rawSqlService = rawSqlService;
        }

        public string? Query { get; set; }

        [RelayCommand]
        private async Task ExecuteQueryAsync() {
            try {
                if (string.IsNullOrWhiteSpace(Query)) return;

                int i = await rawSqlService.ExecuteAsync(Query);

                ResultViewModel = new TextResultViewModel($"Rows affected: {i}");
            } catch (Exception e) {
                ResultViewModel = new TextResultViewModel($"Exception occured: {e.Message}");
            }
        }

        [RelayCommand]
        private async Task QueryAsync() {
            if (string.IsNullOrWhiteSpace(Query)) return;

            try {
                RawSqlQueryResult result = await rawSqlService.QueryAsync(Query);

                TableResultViewModel vm = new(result.Columns, result.Rows);
                ResultViewModel = vm;
            } catch (Exception e) {
                ResultViewModel = new TextResultViewModel($"Exception occured: {e.Message}");
            }
        }
    }
}
