using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FinancialTracker {
    public static class ConfigHelper {
        public static IConfigurationBuilder UseCommonConfiguration(this IConfigurationBuilder configBuilder) {
            configBuilder.AddInMemoryCollection([
                new("DatabasePath", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "finances.db"))
            ]);

            return configBuilder;
        }

        public static string GetDatabasePath(this IConfiguration config) {
            return config["DatabasePath"] ?? throw new Exception("DatebasePath is not configured.");
        }
    }
}
