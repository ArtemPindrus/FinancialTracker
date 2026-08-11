using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace FinancialTracker.Models {
    // TODO: rename to viewmodel
    public partial class FinanceRecordDto : ObservableObject {
        [ObservableProperty]
        public partial string Name { get; set; }

        [ObservableProperty]
        public partial double Amount { get; set; }

        [ObservableProperty]
        public partial DateOnly Date { get; set; }

        [ObservableProperty]
        public partial bool IsDeleted { get; set; }

        [ObservableProperty]
        public partial List<string> Tags { get; set; }

        public bool IsModified { get; private set; }

        public int Id { get; }

        public bool IsAdded { get; }


        public FinanceRecordDto(int id, string name, double amount, DateOnly date, IEnumerable<string> tags, 
            bool isAdded = false) {
            this.Id = id;
            Name = name;
            Amount = amount;
            Date = date;
            IsAdded = isAdded;
            Tags = new(tags);
        }

        public FinanceRecordDto() : this(-1, string.Empty, 0, DateOnly.FromDateTime(DateTime.Now), [], true) {
        }
    }
}
