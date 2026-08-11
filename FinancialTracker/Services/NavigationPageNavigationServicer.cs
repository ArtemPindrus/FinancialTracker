using Avalonia.Controls;
using FinancialTracker.ViewModels;
using System.Threading.Tasks;

namespace FinancialTracker.Services {
    public class NavigationPageNavigationServicer : INavigationService {
        readonly NavigationPage navigationPage;

        public NavigationPageNavigationServicer(NavigationPage navigationPage) {
            this.navigationPage = navigationPage;
        }

        public async Task NavigateToAsync(ViewModelBase vm) {
            ContentPage c = new() {
                Content = vm,
            };

            await navigationPage.PushAsync(c);
        }
    }
}
