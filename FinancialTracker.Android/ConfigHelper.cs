using Microsoft.Extensions.Configuration;
using System.IO;

namespace FinancialTracker.Android {
    public static class ConfigHelper {
        public static IConfigurationBuilder UseCommonConfiguration(this IConfigurationBuilder cb) {
            string directory = global::Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath;

            cb.AddInMemoryCollection([
                new("DatabasePath", Path.Combine(directory, "finances.db"))
            ]);

            return cb;
        }
    }
}
