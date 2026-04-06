using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FinancialTracker.Android {
    public static class ConfigHelper {
        public static IConfigurationBuilder UseCommonConfiguration(this IConfigurationBuilder cb) {
            string directory = AppDomain.CurrentDomain.BaseDirectory;

            cb.AddInMemoryCollection([
                new("DatabasePath", Path.Combine(directory, "finances.db"))
            ]);

            return cb;
        }
    }
}
