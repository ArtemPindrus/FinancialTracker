using FinancialTracker.Models;
using FinancialTracker.ViewModels;
using System.Collections.Generic;

namespace FinancialTracker.Commands {
    public class RemoveTagFromSelectedRecordsCommand : IUndoableCommand {
        readonly FinancesViewModel vm;
        readonly string tag;
        readonly List<FinanceRecordDto> modifiedFinances = new();

        public bool IsReversable => modifiedFinances.Count > 0;

        public RemoveTagFromSelectedRecordsCommand(string tag, FinancesViewModel vm) {
            this.tag = tag;
            this.vm = vm;
        }

        public void Execute() {
            if (modifiedFinances.Count == 0) {
                foreach (var f in vm.SelectedFinances) {
                    if (f.Tags.Contains(tag)) {
                        f.Tags.Remove(tag);
                        modifiedFinances.Add(f);
                    }
                }
            } else {
                foreach (var f in modifiedFinances) {
                    f.Tags.Remove(tag);
                }
            }
        }

        public void Unexecute() {
            foreach (var f in modifiedFinances) {
                f.Tags.Add(tag);
            }
        }
    }
}
