using FinancialTracker.Models;
using FinancialTracker.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace FinancialTracker.Commands {
    public class AddTagFromSelectedRecordsCommand : IUndoableCommand {
        readonly FinancesViewModel vm;
        readonly string tag;
        readonly List<FinanceRecordDto> finances = new();

        public bool IsReversable => finances.Count > 0;

        public AddTagFromSelectedRecordsCommand(string tag, FinancesViewModel vm) {
            this.tag = tag;
            this.vm = vm;
        }

        public void Execute() {
            foreach (var f in vm.SelectedFinances) {
                if (!f.Tags.Contains(tag)) {
                    f.Tags.Add(tag);
                    finances.Add(f);
                }
            }
        }

        public void Unexecute() {
            foreach (var f in finances) {
                f.Tags.Remove(tag);
            }
        }
    }
}
