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
    public partial class FinancesViewModel : ViewModelBase {
        readonly IDbContextFactory<AppDbContext> dbContextFactory;
        readonly CommandHistory commandHistory;

        public ICommand UndoCommand => commandHistory.UndoCommand;
        public ICommand RedoCommand => commandHistory.RedoCommand;

        public ObservableCollection<FinanceRecordDto> Finances { get; } = [];

        public IEnumerable<FinanceRecordDto>? SelectedFinances => SelectedFinancesBind?.Cast<FinanceRecordDto>();

        public IList? SelectedFinancesBind { get; set; }

        public List<string> Tags { get; private set; }

        public List<MenuItem> AddTagsMenuItems { 
            get; 
        } = [];

        public List<MenuItem> RemoveTagsMenuItems {
            get;
        } = [];

        public FinancesViewModel(IDbContextFactory<AppDbContext> dbContextFactory) {
            this.dbContextFactory = dbContextFactory;
            commandHistory = new CommandHistory();

            // TODO: move into async operation
            using AppDbContext dbContext = dbContextFactory.CreateDbContext();

            Tags = dbContext.Tags.Select(x => x.Name).ToList();

            InitializeMenuItems(AddTagsMenuItems, AddTagToSelectedRecordsCommand);
            InitializeMenuItems(RemoveTagsMenuItems, RemoveTagFromSelectedRecordsCommand);
        }

        public async Task PopulateTableAsync() {
            await using AppDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            Finances.Clear();

            var finances = dbContext.Finances.Include(x => x.Tags)
                .AsAsyncEnumerable();


            await foreach (var i in finances) {
                Finances.Add(i.ToDto());
            }
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

        [RelayCommand]
        private async Task SaveChangesAsync() {
            using (AppDbContext dbContext = await dbContextFactory.CreateDbContextAsync()) {
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
            }
            
            await PopulateTableAsync();

            commandHistory.Clear();
        }

        [RelayCommand]
        private async Task RollbackAsync() {
            await PopulateTableAsync();
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
