
namespace FinancialTracker.ViewModels {
    public class MainNavigationPaneViewModel : ViewModelBase {
        /// <summary>
        /// Check if the view model can be safely closed. 
        /// If not, return false and set the message to a user-friendly message explaining why it cannot be closed.
        /// 
        /// By default returns true.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual bool CheckCanSafelyClose(out string message) {
            message = string.Empty;
            return true;
        }
    }
}
