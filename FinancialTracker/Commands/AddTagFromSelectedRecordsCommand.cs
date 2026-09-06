using FinancialTracker.Models;
using FinancialTracker.ViewModels;
using System.Collections.Generic;

namespace FinancialTracker.Commands {
    public class AddTagFromSelectedRecordsCommand : UndoableCommand {
        readonly string tag;
        readonly IEnumerable<FinanceRecordDto> selectedFinances;
        readonly List<FinanceRecordDto> modifiedFinances = [];

        public override bool IsReversable => modifiedFinances.Count > 0;

        public AddTagFromSelectedRecordsCommand(string tag, IEnumerable<FinanceRecordDto> selectedFinances) {
            this.tag = tag;
            this.selectedFinances = selectedFinances;
        }

        public override void Execute(object? p) {
            if (modifiedFinances.Count == 0) {
                foreach (var f in selectedFinances) {
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
