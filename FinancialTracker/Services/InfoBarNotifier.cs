using FluentAvalonia.UI.Controls;
using System;

namespace FinancialTracker.Services {
    public class InfoBarNotifier : IErrorNotifier {
        readonly InfoBar infoBar;

        public InfoBarNotifier(InfoBar infoBar) {
            this.infoBar = infoBar;
        }

        public void Info(string message) => Log(message, InfoBarSeverity.Informational);

        public void Error(string message) {
            Log(message, InfoBarSeverity.Error);
        }

        public void Warning(string message) {
            Log(message, InfoBarSeverity.Warning);
        }

        public void Log(string message, InfoBarSeverity severity) {
            infoBar.IsOpen = true;

            infoBar.Message = $"[{DateTime.Now:HH:mm:ss}] {message}";
            infoBar.Severity = severity;
        }
    }
}
