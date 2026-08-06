using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using DialogHostAvalonia;
using FinancialTracker.Commands;
using FinancialTracker.Models;
using FinancialTracker.ViewModels;
using FinancialTracket.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinancialTracker.StateMachines {
    public partial class FinancesViewModelStateMachine : BaseStateMachine<FinancesViewModelStateMachine.EventId>, IDisposable {
        readonly FinancesViewModel vm;
        readonly IDbContextFactory<AppDbContext> dbContextFactory;

        CancellationTokenSource? populateTableCts;

        [ObservableProperty]
        public partial List<string>? Tags { get; private set; }

        [ObservableProperty]
        public partial bool IsViewEnabled { get; private set; } = true;

        public ObservableCollection<FinanceRecordDto> Finances { get; } = [];

        public CommandHistory CommandHistory { get; }


        public FinancesViewModelStateMachine(FinancesViewModel vm, IDbContextFactory<AppDbContext> dbContextFactory) {
            this.vm = vm;
            this.dbContextFactory = dbContextFactory;
            CommandHistory = new();
        }

        public void Dispose() {
            populateTableCts?.Cancel();
        }

        protected override void DispatchEventImpl(EventId eventId) => DispatchEvent(eventId);

        async void OnSavingEnter() {
            _ = DialogHost.Show(new SavingDatabaseViewModel());

            using (AppDbContext dbContext = dbContextFactory.CreateDbContext()) {
                await dbContext.SaveModificationsAsync(Finances);
            }

            CommandHistory.Clear();

            DialogHost.Close(null);

            DispatchEventNotify(EventId.SAVESUCCESS);
        }

        async void OnPopulatingEnter() {
            IsViewEnabled = false;

            populateTableCts = new();

            using AppDbContext dbContext = dbContextFactory.CreateDbContext();

            Finances.Clear();

            await Task.Run(() => {
                Tags = dbContext.Tags.Select(x => x.Name).ToList();

                Dispatcher.UIThread.Invoke(() => {
                    InitializeMenuItems(vm.AddTagsMenuItems, vm.AddTagToSelectedRecordsCommand);
                    InitializeMenuItems(vm.RemoveTagsMenuItems, vm.RemoveTagFromSelectedRecordsCommand);
                });

                var finances = dbContext.Finances
                    .Include(x => x.Tags)
                    .ToList();

                foreach (var i in finances) {
                    if (populateTableCts.IsCancellationRequested) return;

                    FinanceRecordDto item = i.ToDto();

                    Dispatcher.UIThread.Invoke(() => Finances.Add(item));
                }
            });

            DispatchEventNotify(EventId.POPULATESUCCESS);
        }

        void OnPopulatingExit() {
            IsViewEnabled = true;
            populateTableCts?.Cancel();
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
    }
}
