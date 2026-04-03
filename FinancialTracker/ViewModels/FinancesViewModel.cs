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
    public partial class FinancesViewModel : ViewModelBase {
        private readonly AppDbContext dbContext;

        public ObservableCollection<FinanceRecordDto> Finances { get; } = [];

        public IList SelectedFinances { 
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
            var finances = dbContext.Finances.Select(x => new FinanceRecordDto(x.Id,
                x.Name,
                x.Amount,
                x.Date,
                x.Tags.Select(t => t.Name).ToList())
            ).ToList();

            Finances.Clear();

            foreach (var i in finances) {
                Finances.Add(i);
            }
        }

        [RelayCommand]
        private void AddTagToSelectedRecords(string tag) {
            foreach (FinanceRecordDto f in SelectedFinances) {
                f.Tags.Add(tag);
            }
        }

        [RelayCommand]
        private void RemoveTagFromSelectedRecords(string tag) {
            foreach (FinanceRecordDto f in SelectedFinances) {
                f.Tags.Remove(tag);
            }
        }

        [RelayCommand]
        private async Task SaveChangesAsync() {
            // DELETE entities that were removed from the list
            // find modified entities and update them
            var modified = Finances.Where(x => x.IsModified);

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

            await CommitChanges();
        }

        [RelayCommand]
        private async Task DeleteRecordAsync() {
            foreach (FinanceRecordDto i in SelectedFinances) {
                dbContext.Finances.Remove(dbContext.Finances.Where(x => x.Id == i.Id).Single());
            }

            await CommitChanges();
        }

        [RelayCommand]
        private async Task AddDefaultRecordAsync() {
            Finance f = new() { Name = "", Amount = 0, Date = DateOnly.FromDateTime(DateTime.Now), Tags = []};

            dbContext.Add(f);
            await CommitChanges();
        }

        private async Task CommitChanges() {
            await dbContext.SaveChangesAsync();

            PopulateTable();
        }
    }
}
