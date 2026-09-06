namespace FinancialTracker.Domain {
    public interface IRawSqlService {
        /// <summary>
        /// Execute SQL command that does not return any data.
        /// </summary>
        /// <param name="sql"></param>
        Task<int> ExecuteAsync(string sql);

        /// <summary>
        /// Execute SQL query that returns generic data.
        /// </summary>
        /// <param name="sql"></param>
        Task<RawSqlQueryResult> QueryAsync(string sql);
    }

    public struct RawSqlQueryResult {
        public string[] Columns { get; }
        public IEnumerable<string[]> Rows { get; }

        public RawSqlQueryResult(string[] columns, IEnumerable<string[]> rows) {
            Columns = columns;
            Rows = rows;
        }
    }
}
