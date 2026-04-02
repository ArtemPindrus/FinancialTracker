using System.Collections.Generic;

namespace FinancialTracker.ViewModels {
    public class TableResultViewModel : ViewModelBase {
        public string[] Columns { get; }

        public List<string[]> Data { get; }

        public TableResultViewModel(string[] columns, List<string[]> data) {
            Columns = columns;
            Data = data;
        }
    }
}
