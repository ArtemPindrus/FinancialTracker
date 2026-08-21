using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialTracker.Commands {
    public interface ICommandInvoker {
        void Execute(IUndoableCommand command, object? parameter);

        void Undo();

        void Redo();

        void Clear();
    }
}
