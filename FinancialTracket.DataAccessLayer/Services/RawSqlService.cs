using FinancialTracker.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Data.Common;

namespace FinancialTracket.DataAccessLayer.Services {
    public class RawSqlService : IRawSqlService {
        private readonly IDbContextFactory<AppDbContext> dbContextFactory;

        public RawSqlService(IDbContextFactory<AppDbContext> dbContextFactory) {
            this.dbContextFactory = dbContextFactory;
        }

        public Task<int> ExecuteAsync(string sql) {
            using var dbContext = dbContextFactory.CreateDbContext();

            int result = dbContext.Database.ExecuteSqlRaw(sql);
            return Task.FromResult(result);
        }

        public async Task<RawSqlQueryResult> QueryAsync(string query) {
            using var dbContext = dbContextFactory.CreateDbContext();

            using var con = dbContext.Database.GetDbConnection();
            con.Open();

            using var command = con.CreateCommand();
            command.CommandText = query;

            using var reader = command.ExecuteReader();
            ReadOnlyCollection<DbColumn> dbColumns = reader.GetColumnSchema();
            int cCount = dbColumns.Count;

            List<string[]> rows = [];

            while (await reader.ReadAsync()) {
                string[] row = new string[cCount];

                for (int i = 0; i < dbColumns.Count; i++) {
                    DbColumn? c = dbColumns[i];

                    object colValue = reader[c.ColumnName];
                    row[i] = Convert.ToString(colValue) ?? throw new Exception($"Failed converting column value to string. Value:\n {colValue}.");
                }

                rows.Add(row);
            }

            return new RawSqlQueryResult(dbColumns.Select(c => c.ColumnName).ToArray(), rows);
        }
    }
}
