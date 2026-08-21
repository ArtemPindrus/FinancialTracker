using FinancialTracker.ViewModels;
using System.Threading.Tasks;

namespace FinancialTracker.Services {
    public interface INavigationService {
        Task NavigateToAsync(ViewModelBase vm);

        Task NavigateBackAsync();
    }
}
