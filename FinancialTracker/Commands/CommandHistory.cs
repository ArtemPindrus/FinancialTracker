using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace FinancialTracker.Commands {
    public class CommandHistory : ICommandInvoker {
        readonly Stack<(IUndoableCommand, object?)> executed = new();
        readonly Stack<(IUndoableCommand, object?)> undone = new();

        public int ExecutedCount => executed.Count;
        public int UndoneCount => undone.Count;

        public RelayCommand UndoCommand;
        public RelayCommand RedoCommand;

        public CommandHistory() {
            UndoCommand = new RelayCommand(Undo, () => ExecutedCount > 0);
            RedoCommand = new RelayCommand(Redo, () => UndoneCount > 0);
        }

        public void Execute(IUndoableCommand command, object? parameter = null) {
            command.Execute(parameter);

            if (command.IsReversable) {
                executed.Push((command, parameter));
                undone.Clear();

                NotifyCanExecuteChanged();
            }
        }

        public void Undo() {
            if (executed.Count > 0) {
                (IUndoableCommand c, object? p) cmd = executed.Pop();
                cmd.c.Unexecute();
                undone.Push(cmd);

                NotifyCanExecuteChanged();
            }
        }

        public void Redo() {
            if (undone.Count > 0) {
                (IUndoableCommand c, object? p) cmd = undone.Pop();
                cmd.c.Execute(cmd.p);
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
