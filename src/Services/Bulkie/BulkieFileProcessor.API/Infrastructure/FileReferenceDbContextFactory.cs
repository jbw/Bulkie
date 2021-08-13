using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Bulkie.API.Infrastructure
{
    public class FileReferenceDbContextFactory : IDesignTimeDbContextFactory<FileReferenceContext>
    {
        public FileReferenceContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<FileReferenceContext>();

            optionsBuilder
                .UseNpgsql("host=localhost;database=filereference;user id=bulkie;password=bulkie;port=5435")
                .UseLowerCaseNamingConvention();

            return new FileReferenceContext(optionsBuilder.Options);
        }
    }
}
