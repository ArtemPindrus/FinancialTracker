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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinancialTracker.StateMachines {
    public partial class FinancesViewModelStateMachine : BaseStateMachine<FinancesViewModelStateMachine.EventId> {
        readonly FinancesViewModel vm;
        readonly IDbContextFactory<AppDbContext> dbContextFactory;

        [ObservableProperty]
        public partial List<string>? Tags { get; private set; }

        [ObservableProperty]
        public partial bool IsViewEnabled { get; private set; } = true;

        [ObservableProperty]
        public partial List<FinanceRecordDto>? Finances { get; set; }

        public CommandHistory CommandHistory { get; }


        public FinancesViewModelStateMachine(FinancesViewModel vm, IDbContextFactory<AppDbContext> dbContextFactory) {
            this.vm = vm;
            this.dbContextFactory = dbContextFactory;
            CommandHistory = new();
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

            using AppDbContext dbContext = dbContextFactory.CreateDbContext();

            await Task.Run(() => {
                Tags = dbContext.Tags.Select(x => x.Name).ToList();

                Dispatcher.UIThread.Invoke(() => {
                    InitializeMenuItems(vm.AddTagsMenuItems, vm.AddTagToSelectedRecordsCommand);
                    InitializeMenuItems(vm.RemoveTagsMenuItems, vm.RemoveTagFromSelectedRecordsCommand);
                });

                Finances = dbContext.Finances
                    .Include(x => x.Tags)
                    .Select(x => x.ToDto())
                    .ToList();
            });

            DispatchEventNotify(EventId.POPULATESUCCESS);
        }

        void OnPopulatingExit() {
            IsViewEnabled = true;
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
