using FinancialTracker.Domain.Models;

namespace FinancialTracker.Domain {
    public interface IFinancesService {
        IEnumerable<Finance> GetFinances();

        void UpdateFinances(IEnumerable<Finance> finances);

        void AddFinances(IEnumerable<Finance> finances);

        /// <summary>
        /// Deletes finances from the database based on their IDs.
        /// </summary>
        /// <param name="finances"></param>
        void DeleteFinances(IEnumerable<int> finances);
    }
}
