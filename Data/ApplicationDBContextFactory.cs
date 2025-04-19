using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Registration.Data
{
    public class ApplicationDBContextFactory : IDesignTimeDbContextFactory<ApplicationDBContext>
    {
        public ApplicationDBContext CreateDbContext(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Get the connection string
            var connectionString = configuration["ConnectionStrings:SqlConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The SQL connection string is not configured.");
            }

            // Configure DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDBContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDBContext(optionsBuilder.Options);
        }
    }
}