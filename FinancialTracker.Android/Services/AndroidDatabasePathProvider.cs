using FinancialTracker.DataAccessLayer.Services;
using System;
using System.IO;

namespace FinancialTracker.Android.Services {
    public class AndroidDatabasePathProvider : IDatabasePathProvider {
        public string GetDatabasePath() {
            string? directory = global::Android.App.Application.Context.GetExternalFilesDir(null)?.Path;

            if (directory == null) throw new Exception("Failed to get Android base directory for the database path.");

            return Path.Combine(directory, "finances.db");
        }
    }
}
