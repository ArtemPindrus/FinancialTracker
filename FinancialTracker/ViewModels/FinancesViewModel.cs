using FinancialTracket.DataAccessLayer;
using FinancialTracket.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;

namespace FinancialTracker.ViewModels {
    public class FinancesViewModel : ViewModelBase {
        public ObservableCollection<Finance> Finances { get; }

        public FinancesViewModel(AppDbContext dbContext) {
            Finance[] finances = dbContext.Finances.Include(x => x.Tags).ToArray();

            Finances = new ObservableCollection<Finance>(finances);
        }
    }
}
