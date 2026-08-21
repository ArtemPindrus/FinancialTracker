using FinancialTracker.Models;
using FinancialTracker.ViewModels;
using System.Collections;
using System.Collections.Generic;

namespace FinancialTracker.Commands {
    public class MarkRecordDeletedCommand : UndoableCommand {
        private readonly FinancesViewModel vm;

        List<FinanceRecordDto>? lastMarked;

        public override bool IsReversable => true;

        public MarkRecordDeletedCommand(FinancesViewModel vm) {
            this.vm = vm;
        }

        public override void Execute(object? p) {
            if (lastMarked is null) {
                lastMarked = new();
                lastMarked.AddRange(vm.SelectedFinances);
            }

            foreach (FinanceRecordDto i in lastMarked) {
                i.IsDeleted = !i.IsDeleted;
            }
        }

        public override void Unexecute() {
            if (lastMarked is null) return;

            foreach (FinanceRecordDto i in lastMarked) {
                i.IsDeleted = !i.IsDeleted;
            }

            IList<FinanceRecordDto> selected = vm.SelectedFinances;
            selected.Clear();

            foreach (var m in lastMarked) {
                selected.Add(m);
            }
        }

        public override bool CanExecute(object? parameter) => true;
    }
}
