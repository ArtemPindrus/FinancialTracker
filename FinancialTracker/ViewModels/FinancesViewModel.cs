using Avalonia.Controls;
using Avalonia.Data;
using CommunityToolkit.Mvvm.Input;
using FinancialTracker.Models;
using FinancialTracket.DataAccessLayer;
using FinancialTracket.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinancialTracker.ViewModels {
    public partial class FinancesViewModel : ViewModelBase, IDisposable {
        private readonly AppDbContext dbContext;

        public ObservableCollection<FinanceRecordDto> Finances { get; } = [];

        public IList? SelectedFinances { 
            get; 
            set; 
        }

        public List<string> Tags { get; }

        public List<MenuItem> AddTagsMenuItems { 
            get; 
        } = [];

        public List<MenuItem> RemoveTagsMenuItems {
            get;
        } = [];

        public FinancesViewModel(AppDbContext dbContext) {
            this.dbContext = dbContext;
            Tags = dbContext.Tags.Select(x => x.Name).ToList();

            InitializeTagMenu(AddTagsMenuItems, AddTagToSelectedRecordsCommand);
            InitializeTagMenu(RemoveTagsMenuItems, RemoveTagFromSelectedRecordsCommand);

            PopulateTable();
        }

        public void Dispose() {
            dbContext.Dispose();

            GC.SuppressFinalize(this);
        }

        private void InitializeTagMenu(List<MenuItem> menu, ICommand command) {
            foreach (var t in Tags) {
                MenuItem m = new() {
                    Header = t,
                    Command = command,
                    CommandParameter = t
                };

                menu.Add(m);
            }
        }

        private void PopulateTable() {
            var finances = dbContext.Finances.Include(x => x.Tags).Select(x => new FinanceRecordDto(x))
                .ToList();

            Finances.Clear();

            foreach (var i in finances) {
                Finances.Add(i);
            }
        }

        [RelayCommand]
        private void AddTagToSelectedRecords(string tag) {
            if (SelectedFinances is null) return;

            foreach (FinanceRecordDto f in SelectedFinances) {
                f.Tags.Add(tag);
            }
        }

        [RelayCommand]
        private void RemoveTagFromSelectedRecords(string tag) {
            if (SelectedFinances is null) return;

            foreach (FinanceRecordDto f in SelectedFinances) {
                f.Tags.Remove(tag);
            }
        }

        [RelayCommand]
        private async Task SaveChangesAsync() {
            var modified = Finances.Where(x => x.IsModified);
            var added = Finances.Where(x => x.IsAdded);
            var deleted = Finances.Where(x => x.IsDeleted);

            foreach (var m in modified) {
                var f = dbContext.Finances
                    .Where(x => x.Id == m.Id)
                    .Include(x => x.Tags)
                    .Single();

                f.Name = m.Name;
                f.Amount = m.Amount;
                f.Date = m.Date;
                f.Tags = dbContext.Tags
                    .Where(t => m.Tags.Contains(t.Name))
                    .ToList();
            }

            foreach (var a in added) {
                var f = new Finance() {
                    Name = a.Name,
                    Amount = a.Amount,
                    Date = a.Date,
                    Tags = dbContext.Tags
                        .Where(t => a.Tags.Contains(t.Name))
                        .ToList()
                };
                dbContext.Finances.Add(f);
            }

            foreach (var d in deleted) {
                var f = dbContext.Finances.Where(x => x.Id == d.Id).Single();
                dbContext.Finances.Remove(f);
            }

            await dbContext.SaveChangesAsync();

            PopulateTable();
        }

        [RelayCommand]
        private void Rollback() {
            PopulateTable();
        }

        [RelayCommand]
        private async Task MarkRecordDeletedAsync() {
            if (SelectedFinances is null) return;

            foreach (FinanceRecordDto i in SelectedFinances) {
                i.IsDeleted = !i.IsDeleted;
            }
        }

        [RelayCommand]
        private async Task AddDefaultRecordAsync() {
            Finances.Add(new FinanceRecordDto());
        }
    }
}
