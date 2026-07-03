using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace FinancialTracker.Commands {
    public class CommandHistory : ICommandInvoker {
        readonly Stack<IUndoableCommand> executed = new();
        readonly Stack<IUndoableCommand> undone = new();

        public int ExecutedCount => executed.Count;
        public int UndoneCount => undone.Count;

        public RelayCommand UndoCommand;
        public RelayCommand RedoCommand;

        public CommandHistory() {
            UndoCommand = new RelayCommand(Undo, () => ExecutedCount > 0);
            RedoCommand = new RelayCommand(Redo, () => UndoneCount > 0);
        }

        public void Execute(IUndoableCommand command) {
            command.Execute();

            if (command.IsReversable) {
                executed.Push(command);
                undone.Clear();

                NotifyCanExecuteChanged();
            }
        }

        public void Undo() {
            if (executed.Count > 0) {
                var cmd = executed.Pop();
                cmd.Unexecute();
                undone.Push(cmd);

                NotifyCanExecuteChanged();
            }
        }

        public void Redo() {
            if (undone.Count > 0) {
                var cmd = undone.Pop();
                cmd.Execute();
                executed.Push(cmd);

                NotifyCanExecuteChanged();
            }
        }

        public void Clear() {
            executed.Clear();
            undone.Clear();
            NotifyCanExecuteChanged();
        }

        private void NotifyCanExecuteChanged() {
            UndoCommand.NotifyCanExecuteChanged();
            RedoCommand.NotifyCanExecuteChanged();
        }
    }
}
