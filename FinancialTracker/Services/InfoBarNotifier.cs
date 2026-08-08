using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using System;

namespace FinancialTracker.Services {
    public class InfoBarNotifier : IErrorNotifier {
        readonly FAInfoBar infoBar;

        public InfoBarNotifier(FAInfoBar infoBar) {
            this.infoBar = infoBar;
        }

        public void Info(string message) => Log(message, FAInfoBarSeverity.Informational);

        public void Error(string message) {
            Log(message, FAInfoBarSeverity.Error);
        }

        public void Warning(string message) {
            Log(message, FAInfoBarSeverity.Warning);
        }

        public void Log(string message, FAInfoBarSeverity severity) {
            Dispatcher.UIThread.Invoke(() => {
                infoBar.IsOpen = true;

                infoBar.Message = $"[{DateTime.Now:HH:mm:ss}] {message}";
                infoBar.Severity = severity;
            });
        }
    }
}
