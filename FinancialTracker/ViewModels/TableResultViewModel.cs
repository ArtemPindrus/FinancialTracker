using System.Collections.Generic;

namespace FinancialTracker.ViewModels {
    public class TableResultViewModel : ViewModelBase {
        public string[] Columns { get; }

        public IEnumerable<string[]> Data { get; }

        public TableResultViewModel(string[] columns, IEnumerable<string[]> data) {
            Columns = columns;
            Data = data;
        }
    }
}
