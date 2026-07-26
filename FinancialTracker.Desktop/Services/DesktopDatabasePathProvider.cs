using FinancialTracker.DataAccessLayer.Services;
using System;
using System.IO;

namespace FinancialTracker.Desktop.Services {
    public class DesktopDatabasePathProvider : IDatabasePathProvider {
        public string GetDatabasePath() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "finances.db");
    }
}
