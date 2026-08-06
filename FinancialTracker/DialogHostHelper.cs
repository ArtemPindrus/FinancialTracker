using DialogHostAvalonia;
using System.Threading.Tasks;

namespace FinancialTracker {
    public static class DialogHostHelper {
        private const string MainDialogIdentifier = "MainDialogHost";
        private const string ContentDialogIdentifier = "ContentDialogHost";

        public static Task ShowMainDialog(object? content) {
            return DialogHost.Show(content, MainDialogIdentifier);
        }

        public static void CloseMainDialog() {
            DialogHost.Close(MainDialogIdentifier);
        }

        public static Task ShowContentDialog(object? content) {
            return DialogHost.Show(content, ContentDialogIdentifier);
        }

        public static void CloseContentDialog() {
            DialogHost.Close(ContentDialogIdentifier);
        }
    }
}
