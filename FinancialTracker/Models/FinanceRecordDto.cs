using CommunityToolkit.Mvvm.ComponentModel;
using FinancialTracket.DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace FinancialTracker.Models {
    public partial class FinanceRecordDto : ObservableObject {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private DateOnly date;

        [ObservableProperty]
        private bool isDeleted;

        public bool IsModified { get; private set; }

        public int Id { get; }

        public bool IsAdded { get; }

        public ObservableCollection<string> Tags { get; }

        public FinanceRecordDto(int id, string name, decimal amount, DateOnly date, IEnumerable<string> tags, 
            bool isAdded = false) {
            this.Id = id;
            this.name = name;
            this.amount = amount;
            this.date = date;
            IsAdded = isAdded;
            Tags = new(tags);

            if (!isAdded) {
                PropertyChanged += FinanceRecordDto_PropertyChanged;
                Tags.CollectionChanged += Tags_CollectionChanged;
            }
        }

        public FinanceRecordDto() : this(-1, string.Empty, 0, DateOnly.FromDateTime(DateTime.Now), [], true) {
        }

        private void Tags_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
            OnChanged();
        }

        private void FinanceRecordDto_PropertyChanged(object? sender, PropertyChangedEventArgs e){
            OnChanged();
        }

        private void OnChanged() {
            if (IsDeleted) return;

            IsModified = true;
        }
    }
}
