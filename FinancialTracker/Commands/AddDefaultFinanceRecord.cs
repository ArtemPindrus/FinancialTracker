using FinancialTracker.Models;
using System.Collections.Generic;

namespace FinancialTracker.Commands {
    public class AddDefaultFinanceRecord : UndoableCommand {
        readonly IList<FinanceRecordDto> finances;

        FinanceRecordDto? lastAddedRecord;

        public override bool IsReversable => true;

        public AddDefaultFinanceRecord(IList<FinanceRecordDto> finances) {
            this.finances = finances;
        }

        public override void Execute(object? p) {
            lastAddedRecord = new();
            finances.Add(lastAddedRecord);
        }

        public override void Unexecute() {
            if (lastAddedRecord != null) {
                finances.Remove(lastAddedRecord);
                lastAddedRecord = null;
            }
        }

        public override bool CanExecute(object? parameter) => true;
    }
}
