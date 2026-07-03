using FinancialTracker.Models;
using FinancialTracker.ViewModels;
using System.Collections.Generic;

namespace FinancialTracker.Commands {
    public class RemoveTagFromSelectedRecordsCommand : IUndoableCommand {
        readonly FinancesViewModel vm;
        readonly string tag;
        readonly List<FinanceRecordDto> removed = new();

        public bool IsReversable => removed.Count > 0;

        public RemoveTagFromSelectedRecordsCommand(string tag, FinancesViewModel vm) {
            this.tag = tag;
            this.vm = vm;
        }

        public void Execute() {
            foreach (var f in vm.SelectedFinances) {
                if (f.Tags.Contains(tag)) {
                    f.Tags.Remove(tag);
                    removed.Add(f);
                }
            }
        }

        public void Unexecute() {
            foreach (var f in removed) {
                f.Tags.Add(tag);
            }
        }
    }
}
