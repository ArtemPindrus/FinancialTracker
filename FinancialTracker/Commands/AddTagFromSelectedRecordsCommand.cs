using FinancialTracker.Models;
using FinancialTracker.ViewModels;
using System.Collections.Generic;

namespace FinancialTracker.Commands {
    public class AddTagFromSelectedRecordsCommand : UndoableCommand {
        readonly FinancesViewModel vm;
        readonly string tag;
        readonly List<FinanceRecordDto> modifiedFinances = new();

        public override bool IsReversable => modifiedFinances.Count > 0;

        public AddTagFromSelectedRecordsCommand(string tag, FinancesViewModel vm) {
            this.tag = tag;
            this.vm = vm;
        }

        public override void Execute(object? p) {
            if (modifiedFinances.Count == 0) {
                foreach (var f in vm.SelectedFinances) {
                    if (!f.Tags.Contains(tag)) {
                        f.Tags.Add(tag);
                        modifiedFinances.Add(f);
                    }
                }
            } else {
                foreach (var f in modifiedFinances) {
                    f.Tags.Add(tag);
                }
            }
        }

        public override void Unexecute() {
            foreach (var f in modifiedFinances) {
                f.Tags.Remove(tag);
            }
        }

        public override bool CanExecute(object? parameter) => true;
    }
}
