using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinancialTracker.ViewModels {
    public partial class FinancesViewModel : MainNavigationPaneViewModel, IDisposable {
        readonly IDbContextFactory<AppDbContext> dbContextFactory;
        readonly CommandHistory commandHistory;
        readonly CancellationTokenSource populateTableCts;

        [ObservableProperty]
        public partial List<string>? Tags { get; private set; }

        public ICommand UndoCommand => commandHistory.UndoCommand;
        public ICommand RedoCommand => commandHistory.RedoCommand;

        public ObservableCollection<FinanceRecordDto> Finances { get; } = [];

        public IEnumerable<FinanceRecordDto>? SelectedFinances => SelectedFinancesBind?.Cast<FinanceRecordDto>();

        public IList? SelectedFinancesBind { get; set; }

        public ObservableCollection<MenuItem> AddTagsMenuItems { 
            get; 
        } = [];

        public ObservableCollection<MenuItem> RemoveTagsMenuItems {
            get;
        } = [];

        public FinancesViewModel(IDbContextFactory<AppDbContext> dbContextFactory) {
            this.dbContextFactory = dbContextFactory;
            commandHistory = new CommandHistory();
            populateTableCts = new();
        }

        public void Dispose() {
            populateTableCts.Cancel();
        }

        public override bool CheckCanSafelyClose(out string message) {
            populateTableCts.Cancel();

            if (Finances.Any(f => f.IsModified) || Finances.Any(x => x.IsAdded)) {
                message = "There are unsaved changes. Are you sure you want to close?";
                return false;
            } else {
                message = string.Empty;
                return true;
            }
        }

        public override async Task Initialize() {
            await PopulateTableAsync();
        }

        public async Task PopulateTableAsync() {
            await using AppDbContext dbContext = await dbContextFactory.CreateDbContextAsync(populateTableCts.Token);

            Finances.Clear();

            await Task.Run(() => {
                Tags = dbContext.Tags.Select(x => x.Name).ToList();

                Dispatcher.UIThread.Invoke(() => {
                    InitializeMenuItems(AddTagsMenuItems, AddTagToSelectedRecordsCommand);
                    InitializeMenuItems(RemoveTagsMenuItems, RemoveTagFromSelectedRecordsCommand);
                });

                var finances = dbContext.Finances
                    .Include(x => x.Tags)
                    .ToList();

                foreach (var i in finances) {
                    if (populateTableCts.IsCancellationRequested) return;

                    FinanceRecordDto item = i.ToDto();
                    
                    Dispatcher.UIThread.Invoke(() => Finances.Add(item));
                }
            }, populateTableCts.Token);
        }

        private void InitializeMenuItems(IList<MenuItem> menu, ICommand command) {
            if (Tags is null) return;

            menu.Clear();

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
        private void MarkRecordDeleted() {
            commandHistory.Execute(new MarkRecordDeletedCommand(this));
        }

        [RelayCommand]
        private void AddDefaultRecord() {
            commandHistory.Execute(new AddDefaultFinanceRecord(Finances));
        }

        
    }
}
