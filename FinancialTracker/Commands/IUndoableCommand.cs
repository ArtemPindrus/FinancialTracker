using System.Windows.Input;

namespace FinancialTracker.Commands {
    public interface IUndoableCommand {
        bool IsReversable { get; }

        void Execute();
        void Unexecute();
    }
}
