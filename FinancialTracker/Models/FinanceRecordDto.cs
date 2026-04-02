using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace FinancialTracker.Models {
    public partial class FinanceRecordDto : ObservableObject {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private DateOnly date;
        
        public bool IsModified { get; private set; }

        public int Id { get; }

        public ObservableCollection<string> Tags { get; }

        public FinanceRecordDto(int id, string name, decimal amount, DateOnly date, IEnumerable<string> tags) {
            this.Id = id;
            this.name = name;
            this.amount = amount;
            this.date = date;
            Tags = new(tags);

            PropertyChanged += FinanceRecordDto_PropertyChanged;
            Tags.CollectionChanged += Tags_CollectionChanged;
        }

        private void Tags_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
            OnChanged();
        }

        private void FinanceRecordDto_PropertyChanged(object? sender, PropertyChangedEventArgs e){
            OnChanged();
        }

        private void OnChanged() {
            IsModified = true;
        }
    }
}
