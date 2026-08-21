using System;
using System.Windows.Input;

namespace FinancialTracker.Commands {
    public interface IUndoableCommand : ICommand {
        bool IsReversable { get; }

        void Unexecute();
    }

    public abstract class UndoableCommand : IUndoableCommand {
        public event EventHandler? CanExecuteChanged;

        public abstract bool IsReversable { get; }

        public abstract bool CanExecute(object? parameter);

        public abstract void Execute(object? parameter);

        public abstract void Unexecute();
    }
}
