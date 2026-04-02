using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialTracker.Views;
using FinancialTracket.DataAccessLayer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialTracker.ViewModels {
    public partial class RawQueryViewModel : ViewModelBase {
        private readonly AppDbContext dbContext;

        [ObservableProperty]
        private ViewModelBase? resultViewModel;

        public RawQueryViewModel(AppDbContext dbContext) {
            this.dbContext = dbContext;
        }

        public string? Query { get; set; }

        [RelayCommand]
        private async Task ExecuteQueryAsync() {
            try {
                if (string.IsNullOrWhiteSpace(Query)) return;

                int i = await dbContext.Database.ExecuteSqlRawAsync(Query);

                ResultViewModel = new TextResultViewModel($"Rows affected: {i}");
            } catch (Exception e) {
                ResultViewModel = new TextResultViewModel($"Exception occured: {e.Message}");
            }
        }

        [RelayCommand]
        private async Task QueryAsync() {
            try {
                using var con = dbContext.Database.GetDbConnection();
                await con.OpenAsync();

                using var command = con.CreateCommand();
                command.CommandText = Query;

                using var reader = await command.ExecuteReaderAsync();
                ReadOnlyCollection<DbColumn> dbColumns = await reader.GetColumnSchemaAsync();
                int cCount = dbColumns.Count;

                List<string[]> rows = [];

                while (await reader.ReadAsync()) {
                    string[] row = new string[cCount];

                    for (int i = 0; i < dbColumns.Count; i++) {
                        DbColumn? c = dbColumns[i];

                        row[i] = Convert.ToString(reader[c.ColumnName]);
                    }

                    rows.Add(row);
                }

                TableResultViewModel vm = new(dbColumns.Select(c => c.ColumnName).ToArray(), rows);
                ResultViewModel = vm;
            } catch (Exception e) {
                ResultViewModel = new TextResultViewModel($"Exception occured: {e.Message}");
            }
        }
    }
}
