using FinancialTracker.Models;
using System.Collections.Generic;

namespace FinancialTracker.Commands {
    public class AddFinanceCommand : UndoableCommand {
        private readonly ICollection<FinanceRecordDto> finances;
        private readonly FinanceRecordDto record;

        public override bool IsReversable => true;

        public AddFinanceCommand(ICollection<FinanceRecordDto> finances, FinanceRecordDto record) {
            this.finances = finances;
            this.record = record;
        }

        public override bool CanExecute(object? parameter) => true;

        public override void Execute(object? parameter) {
            finances.Add(record);
        }

        public override void Unexecute() {
            finances.Remove(record);
        }
    }
}
