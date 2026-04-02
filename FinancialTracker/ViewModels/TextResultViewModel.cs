namespace FinancialTracker.ViewModels {
    public class TextResultViewModel : ViewModelBase {
        public string Text { get; }

        public TextResultViewModel(string text) {
            Text = text;
        }
    }
}
