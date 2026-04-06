using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FinancialTracker {
    public static class AppConfig {
        public static IConfigurationBuilder DefaultConfigurationBuilder { get; } = new ConfigurationBuilder();

        public static IConfiguration BuildDefaultConfiguration() => DefaultConfigurationBuilder.Build();

        public static string GetDatabasePath(this IConfiguration config) {
            return config["DatabasePath"] ?? throw new Exception("DatebasePath is not configured.");
        }
    }
}
