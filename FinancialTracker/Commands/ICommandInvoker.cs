using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialTracker.Commands {
    public interface ICommandInvoker {
        void Execute(IUndoableCommand command);

        void Undo();

        void Redo();

        void Clear();
    }
}
