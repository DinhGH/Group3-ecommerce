using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.eShopWeb.Infrastructure;

public static class Dependencies
{
    public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        bool useOnlyInMemoryDatabase = false;
        if (configuration["UseOnlyInMemoryDatabase"] != null)
        {
            useOnlyInMemoryDatabase = bool.Parse(configuration["UseOnlyInMemoryDatabase"]!);
        }

        if (useOnlyInMemoryDatabase)
        {
            services.AddDbContext<CatalogContext>(c =>
               c.UseInMemoryDatabase("Catalog"));
         
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseInMemoryDatabase("Identity"));
        }
        else
        {
            // use real database
            // Support both SQL Server and SQLite
            var catalogConnection = configuration.GetConnectionString("CatalogConnection");
            var identityConnection = configuration.GetConnectionString("IdentityConnection");
            
            // Check if using SQLite (for Render.com deployment)
            if (catalogConnection?.Contains("Data Source=") == true && !catalogConnection.Contains("Server="))
            {
                services.AddDbContext<CatalogContext>(c =>
                    c.UseSqlite(catalogConnection));
                services.AddDbContext<AppIdentityDbContext>(options =>
                    options.UseSqlite(identityConnection));
            }
            else
            {
                // SQL Server (local development)
                services.AddDbContext<CatalogContext>(c =>
                    c.UseSqlServer(catalogConnection));
                services.AddDbContext<AppIdentityDbContext>(options =>
                    options.UseSqlServer(identityConnection));
            }
        }
    }
}
