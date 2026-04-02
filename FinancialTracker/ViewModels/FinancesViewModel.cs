using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialTracker.Models;
using FinancialTracket.DataAccessLayer;
using FinancialTracket.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialTracker.ViewModels {
    public partial class FinancesViewModel : ViewModelBase {
        private readonly AppDbContext dbContext;

        public ObservableCollection<FinanceRecordDto> Finances { get; } = new();

        public List<string> Tags { get; }

        public FinancesViewModel(AppDbContext dbContext) {
            this.dbContext = dbContext;
            Tags = dbContext.Tags.Select(x => x.Name).ToList();

            PopulateTable();
        }

        private void PopulateTable() {
            var finances = dbContext.Finances.Select(x => new FinanceRecordDto(x.Id,
                x.Name,
                x.Amount,
                x.Date,
                x.Tags.Select(t => t.Name).ToList())
            ).ToList();

            Finances.Clear();

            foreach (var i in finances) {
                Finances.Add(i);
            }
        }

        [RelayCommand]
        private async Task SaveChangesAsync() {
            // DELETE entities that were removed from the list
            // find modified entities and update them
            var modified = Finances.Where(x => x.IsModified);

            foreach (var m in modified) {
                var f = dbContext.Finances
                    .Where(x => x.Id == m.Id)
                    .Include(x => x.Tags)
                    .Single();

                f.Name = m.Name;
                f.Amount = m.Amount;
                f.Date = m.Date;
                f.Tags = dbContext.Tags
                    .Where(t => m.Tags.Contains(t.Name))
                    .ToArray();
            }

            await CommitChanges();
        }

        [RelayCommand]
        private async Task DeleteRecordAsync(FinanceRecordDto finance) {
            dbContext.Finances.Remove(dbContext.Finances.Where(x => x.Id == finance.Id).Single());

            await CommitChanges();
        }

        [RelayCommand]
        private async Task AddDefaultRecordAsync() {
            Finance f = new() { Name = "", Amount = 0, Date = DateOnly.FromDateTime(DateTime.Now), Tags = []};

            dbContext.Add(f);
            await CommitChanges();
        }

        private async Task CommitChanges() {
            await dbContext.SaveChangesAsync();

            PopulateTable();
        }
    }
}
