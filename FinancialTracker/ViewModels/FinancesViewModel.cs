using CommunityToolkit.Mvvm.Input;
using FinancialTracket.DataAccessLayer;
using FinancialTracket.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FinancialTracker.ViewModels {
    public partial class FinancesViewModel : ViewModelBase {
        private readonly AppDbContext dbContext;

        public ObservableCollection<Finance> Finances { get; }

        public FinancesViewModel(AppDbContext dbContext) {
            Finance[] finances = dbContext.Finances.Include(x => x.Tags).ToArray();

            Finances = new ObservableCollection<Finance>(finances);
            this.dbContext = dbContext;
        }

        [RelayCommand]
        private async Task SaveChangesAsync() {
            await dbContext.SaveChangesAsync();
        }

        [RelayCommand]
        private void DeleteRecord(Finance finance) {
            dbContext.Finances.Remove(finance);
            Finances.Remove(finance);
        }
    }
}
