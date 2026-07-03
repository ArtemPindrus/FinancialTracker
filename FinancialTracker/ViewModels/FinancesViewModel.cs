using Avalonia.Controls;
using Avalonia.Data;
using CommunityToolkit.Mvvm.Input;
using FinancialTracker.Commands;
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
        readonly AppDbContext dbContext;
        readonly CommandHistory commandHistory;

        public ICommand UndoCommand => commandHistory.UndoCommand;
        public ICommand RedoCommand => commandHistory.RedoCommand;

        public ObservableCollection<FinanceRecordDto> Finances { get; } = [];

        public IEnumerable<FinanceRecordDto>? SelectedFinances => SelectedFinancesBind?.Cast<FinanceRecordDto>();

        public IList? SelectedFinancesBind { get; set; }

        public List<string> Tags { get; }

        public List<MenuItem> AddTagsMenuItems { 
            get; 
        } = [];

        public List<MenuItem> RemoveTagsMenuItems {
            get;
        } = [];

        public FinancesViewModel(AppDbContext dbContext) {
            this.dbContext = dbContext;
            commandHistory = new CommandHistory();

            Tags = dbContext.Tags.Select(x => x.Name).ToList();

            InitializeMenuItems(AddTagsMenuItems, AddTagToSelectedRecordsCommand);
            InitializeMenuItems(RemoveTagsMenuItems, RemoveTagFromSelectedRecordsCommand);

            PopulateTable();
        }

        public void Dispose() {
            dbContext.Dispose();

            GC.SuppressFinalize(this);
        }

        private void InitializeMenuItems(List<MenuItem> menu, ICommand command) {
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
            var finances = dbContext.Finances.Include(x => x.Tags)
                .Select(x => x.ToDto())
                .ToList();

            Finances.Clear();

            foreach (var i in finances) {
                Finances.Add(i);
            }
        }

        [RelayCommand]
        private async Task SaveChangesAsync() {
            var modified = Finances.Where(x => x.IsModified);
            var added = Finances.Where(x => x.IsAdded);
            var deleted = Finances.Where(x => x.IsDeleted);

            foreach (var d in deleted) {
                var f = dbContext.Finances.Where(x => x.Id == d.Id).Single();
                dbContext.Finances.Remove(f);
            }

            foreach (FinanceRecordDto m in modified) {
                Finance f = dbContext.Finances
                    .Where(x => x.Id == m.Id)
                    .Include(x => x.Tags)
                    .Single();

                await dbContext.AddMissingTagsToDatabaseAsync(m);

                DbHelper.SyncDtoToEntity(m, f, dbContext);
            }

            foreach (FinanceRecordDto a in added) {
                await dbContext.AddMissingTagsToDatabaseAsync(a);

                Finance f = a.ToEntity(dbContext);
                dbContext.Finances.Add(f);
            }

            await dbContext.SaveChangesAsync();

            PopulateTable();

            commandHistory.Clear();
        }

        [RelayCommand]
        private void Rollback() {
            PopulateTable();
            commandHistory.Clear();
        }

        [RelayCommand]
        private void AddTagToSelectedRecords(string tag) {
            commandHistory.Execute(new AddTagFromSelectedRecordsCommand(tag, this));

        }

        [RelayCommand]
        private void RemoveTagFromSelectedRecords(string tag) {
            commandHistory.Execute(new RemoveTagFromSelectedRecordsCommand(tag, this));
        }

        [RelayCommand]
        private async Task MarkRecordDeletedAsync() {
            commandHistory.Execute(new MarkRecordDeletedCommand(this));
        }

        [RelayCommand]
        private void AddDefaultRecord() {
            commandHistory.Execute(new AddDefaultFinanceRecord(Finances));
        }
    }
}
