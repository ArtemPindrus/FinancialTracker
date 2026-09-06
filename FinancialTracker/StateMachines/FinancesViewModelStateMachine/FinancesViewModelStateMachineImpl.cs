using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using FinancialTracker.Commands;
using FinancialTracker.Domain;
using FinancialTracker.Domain.Models;
using FinancialTracker.Models;
using FinancialTracker.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinancialTracker.StateMachines {
    public partial class FinancesViewModelStateMachine : BaseStateMachine<FinancesViewModelStateMachine.EventId> {
        readonly FinancesViewModel vm;
        readonly IFinancesService financeService;
        readonly ITagsService tagsService;

        [ObservableProperty]
        public partial List<string> Tags { get; private set; } = [];

        public ObservableCollection<FinanceRecordDto> Finances { get; set; } = [];

        public CommandHistory CommandHistory { get; }


        public FinancesViewModelStateMachine(FinancesViewModel vm, IFinancesService financeService, ITagsService tagsService) {
            this.vm = vm;
            this.financeService = financeService;
            this.tagsService = tagsService;

            CommandHistory = new();
        }

        protected override void DispatchEventImpl(EventId eventId) => DispatchEvent(eventId);

        async void OnSavingEnter() {
            _ = DialogHostHelper.ShowMainDialog(new ProgressRingViewModel("Saving database..."));

            var modified = Finances.Where(x => x.IsModified && !x.IsDeleted);
            var added = Finances.Where(x => x.IsAdded);
            var deleted = Finances.Where(x => x.IsDeleted);

            financeService.DeleteFinances(deleted.Select(x => x.Id));

            List<Finance> finances = modified
                .Select(x => x.ToEntity(tagsService))
                .ToList();

            financeService.UpdateFinances(finances);

            financeService.AddFinances(added.Select(x => x.ToEntity(tagsService)));

            CommandHistory.Clear();

            DispatchEventNotify(EventId.SAVESUCCESS);
        }

        void OnSavingExit() {
            DialogHostHelper.CloseMainDialog();
        }

        async void OnPopulatingEnter() {
            _ = DialogHostHelper.ShowContentDialog(new ProgressRingViewModel("Querying database..."));

            Finances.Clear();

            await Task.Run(() => {
                Tags = tagsService.GetTags()
                    .Select(x => x.Name)
                    .ToList();

                Dispatcher uIThread = Dispatcher.UIThread;
                uIThread.Invoke(() => {
                    InitializeMenuItems(vm.AddTagsMenuItems, vm.AddTagToSelectedRecordsCommand);
                    InitializeMenuItems(vm.RemoveTagsMenuItems, vm.RemoveTagFromSelectedRecordsCommand);
                });

                var newList = financeService.GetFinances()
                    .Select(x => x.ToDto())
                    .ToList();

                uIThread.Invoke(() => {
                    foreach (var item in newList) {
                        Finances.Add(item);
                    }
                });
            });

            DispatchEventNotify(EventId.POPULATESUCCESS);
        }

        void OnPopulatingExit() {
            DialogHostHelper.CloseContentDialog();
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
