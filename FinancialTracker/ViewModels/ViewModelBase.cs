using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace FinancialTracker.ViewModels;

public abstract class ViewModelBase : ObservableObject {
    public virtual Task Initialize() { return Task.CompletedTask; }
}
