using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Bulkie.API.Infrastructure
{
    public class BulkieDbContextFactory : IDesignTimeDbContextFactory<BulkieContext>
    {
        public BulkieContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<BulkieContext>();

            optionsBuilder
                .UseNpgsql("host=localhost;database=bulkie;user id=bulkie;password=bulkie;port=5435")
                .UseLowerCaseNamingConvention();

            return new BulkieContext(optionsBuilder.Options);
        }
    }
}
