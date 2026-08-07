namespace FinancialTracker.Services {
    public interface IErrorNotifier {
        void Info(string message);

        void Error(string message);

        void Warning(string message);
    }
}
