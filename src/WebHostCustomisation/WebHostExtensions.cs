using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostExtensions
    {
        public static IHost MigrateDatabaseContext<TDbContext>(this IHost host) where TDbContext : DbContext
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<TDbContext>();

            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                throw;
            }

            return host;
        }
    }
}
