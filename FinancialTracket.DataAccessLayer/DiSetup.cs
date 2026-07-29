using FinancialTracker.DataAccessLayer.Services;
using FinancialTracket.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialTracket.DataAccessLayer {
    public static class DiSetup {
        public static IServiceCollection SetupDataAccessLayer(this IServiceCollection services) {
            services.AddDbContextFactory<AppDbContext>((sp, options) => {
                string databasePath = sp.GetRequiredService<IDatabasePathProvider>().GetDatabasePath();
                string connectionString = $"Data Source={databasePath};Pooling=False";

                options.UseSqlite(connectionString);
                options.UseSeeding((db, _) => {
                    SeedDatabase((AppDbContext)db);
                });
            });

            return services;
        }

        private static void SeedDatabase(AppDbContext db) {
            // Only seed if database is empty
            if (db.Tags.Any() || db.Finances.Any()) {
                return;
            }

            // Create tags
            var tags = new List<Tag>
            {
                new() { Name = "Groceries" },
                new() { Name = "Utilities" },
                new() { Name = "Entertainment" },
                new() { Name = "Transportation" },
                new() { Name = "Healthcare" },
                new() { Name = "Dining Out" },
                new() { Name = "Shopping" },
                new() { Name = "Subscription" },
                new() { Name = "Insurance" },
                new() { Name = "Salary" },
                new() { Name = "Freelance" },
                new() { Name = "Investment" },
                new() { Name = "Bonus" },
                new() { Name = "Rent" },
                new() { Name = "Phone" },
                new() { Name = "Internet" },
                new() { Name = "Gym" },
                new() { Name = "Books" },
                new() { Name = "Travel" },
                new() { Name = "Home Maintenance" }
            };

            db.Tags.AddRange(tags);
            db.SaveChanges();

            // Create financial records with realistic data
            var finances = new List<Finance>();
            var random = new Random(42); // Fixed seed for reproducibility
            var today = DateOnly.FromDateTime(DateTime.Now);

            // Generate 200+ financial records
            for (int i = 0; i < 200; i++) {
                var daysAgo = random.Next(0, 365); // Last year of data
                var transactionDate = today.AddDays(-daysAgo);

                // Mix of income and expenses
                var isIncome = random.Next(0, 100) < 20; // 20% income

                Finance finance = new()
                {
                    Name = isIncome ? GenerateIncomeDescription(random) : GenerateExpenseDescription(random),
                    Amount = isIncome ? GenerateIncomeAmount(random) : GenerateExpenseAmount(random),
                    Date = transactionDate
                };

                // Assign 1-3 random tags to each transaction
                var tagCount = random.Next(1, 4);
                var selectedTags = tags.OrderBy(_ => random.Next()).Take(tagCount).ToList();
                finance.Tags = selectedTags;

                finances.Add(finance);
            }

            // Add some bulk expenses and income on specific months
            var baseDate = today.AddMonths(-6);
            for (int month = 0; month < 6; month++) {
                var monthDate = baseDate.AddMonths(month);

                // Monthly salary
                finances.Add(new Finance
                {
                    Name = "Monthly Salary",
                    Amount = 5000,
                    Date = monthDate,
                    Tags = new List<Tag> { tags.First(t => t.Name == "Salary") }
                });

                // Rent
                finances.Add(new Finance
                {
                    Name = "Apartment Rent",
                    Amount = -1500,
                    Date = monthDate.AddDays(1),
                    Tags = new List<Tag> { tags.First(t => t.Name == "Rent") }
                });

                // Utilities
                finances.Add(new Finance
                {
                    Name = "Electric Bill",
                    Amount = -(80 + random.Next(-20, 30)),
                    Date = monthDate.AddDays(5),
                    Tags = new List<Tag> { tags.First(t => t.Name == "Utilities") }
                });

                // Internet
                finances.Add(new Finance
                {
                    Name = "Internet Bill",
                    Amount = -60,
                    Date = monthDate.AddDays(6),
                    Tags = new List<Tag> { tags.First(t => t.Name == "Internet") }
                });

                // Grocery shopping (multiple times per month)
                for (int j = 0; j < 4; j++) {
                    finances.Add(new Finance
                    {
                        Name = $"Grocery Shopping #{j + 1}",
                        Amount = -(60 + random.Next(0, 80)),
                        Date = monthDate.AddDays(7 + (j * 7)),
                        Tags = new List<Tag> { tags.First(t => t.Name == "Groceries") }
                    });
                }

                // Gym membership
                finances.Add(new Finance
                {
                    Name = "Gym Membership",
                    Amount = -50,
                    Date = monthDate.AddDays(1),
                    Tags = new List<Tag> { tags.First(t => t.Name == "Gym") }
                });

                // Subscriptions
                finances.Add(new Finance
                {
                    Name = "Streaming Service",
                    Amount = -15,
                    Date = monthDate.AddDays(2),
                    Tags = new List<Tag> { tags.First(t => t.Name == "Subscription") }
                });
            }

            db.Finances.AddRange(finances);
            db.SaveChanges();
        }

        private static string GenerateIncomeDescription(Random random) {
            var descriptions = new[]
            {
                "Freelance Project Payment",
                "Bonus",
                "Refund",
                "Interest Payment",
                "Side Gig Income",
                "Commission",
                "Consulting Fee",
                "Royalty Payment"
            };
            return descriptions[random.Next(descriptions.Length)];
        }

        private static string GenerateExpenseDescription(Random random) {
            var descriptions = new[]
            {
                "Amazon Purchase",
                "Coffee Shop",
                "Gas Station",
                "Restaurant",
                "Movie Tickets",
                "Pharmacy",
                "Bookstore",
                "Clothing Store",
                "Taxi Ride",
                "Pizza Delivery",
                "Hair Salon",
                "Gaming Purchase",
                "Office Supplies",
                "Parking",
                "ATM Withdrawal"
            };
            return descriptions[random.Next(descriptions.Length)];
        }

        private static double GenerateExpenseAmount(Random random) {
            var categories = new[]
            {
                (2, 15),      // Small snacks/coffee
                (15, 50),     // Medium purchases
                (50, 150),    // Larger purchases
                (150, 300)    // Big purchases
            };

            var selectedCategory = categories[random.Next(categories.Length)];
            var amount = -(selectedCategory.Item1 + random.NextDouble() * (selectedCategory.Item2 - selectedCategory.Item1));
            return Math.Round(amount, 2);
        }

        private static double GenerateIncomeAmount(Random random) {
            var categories = new[]
            {
                (50, 150),    // Small income
                (200, 500),   // Medium income
                (500, 1500)   // Larger income
            };

            var selectedCategory = categories[random.Next(categories.Length)];
            var amount = selectedCategory.Item1 + random.NextDouble() * (selectedCategory.Item2 - selectedCategory.Item1);
            return Math.Round(amount, 2);
        }
    }
}
