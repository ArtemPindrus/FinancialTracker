using FinancialTracker.Models;
using FinancialTracker.ViewModels;
using System.Collections;
using System.Collections.Generic;

namespace FinancialTracker.Commands {
    public class MarkRecordDeletedCommand : IUndoableCommand {
        private readonly FinancesViewModel vm;

        List<FinanceRecordDto>? lastMarked;

        public bool IsReversable => true;

        public MarkRecordDeletedCommand(FinancesViewModel vm) {
            this.vm = vm;
        }

        public void Execute() {
            if (lastMarked is null) {
                lastMarked = new();
                lastMarked.AddRange(vm.SelectedFinances);
            }

            foreach (FinanceRecordDto i in lastMarked) {
                i.IsDeleted = !i.IsDeleted;
            }
        }

        public void Unexecute() {
            if (lastMarked is null) return;

            foreach (FinanceRecordDto i in lastMarked) {
                i.IsDeleted = !i.IsDeleted;
            }

            IList? selected = vm.SelectedFinancesBind;
            selected.Clear();

            foreach (var m in lastMarked) {
                selected.Add(m);
            }
        }
    }
}
