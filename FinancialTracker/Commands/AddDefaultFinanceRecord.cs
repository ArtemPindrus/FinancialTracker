using FinancialTracker.Models;
using System;
using System.Collections.Generic;

namespace FinancialTracker.Commands {
    public class AddDefaultFinanceRecord : IUndoableCommand {
        readonly IList<FinanceRecordDto> finances;

        FinanceRecordDto? lastAddedRecord;

        public bool IsReversable => true;

        public AddDefaultFinanceRecord(IList<FinanceRecordDto> finances) {
            this.finances = finances;
        }

        public void Execute() {
            lastAddedRecord = new();
            finances.Add(lastAddedRecord);
        }

        public void Unexecute() {
            if (lastAddedRecord != null) {
                finances.Remove(lastAddedRecord);
                lastAddedRecord = null;
            }
        }
    }
}
