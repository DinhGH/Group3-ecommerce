using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.Infrastructure.Identity;

namespace Microsoft.eShopWeb.Web.Middleware;

public class DatabaseInitializationMiddleware
{
    private readonly RequestDelegate _next;
    private static bool _initialized = false;
    private static readonly object _lock = new object();

    public DatabaseInitializationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, 
        CatalogContext catalogContext, 
        AppIdentityDbContext identityContext,
        ILogger<DatabaseInitializationMiddleware> logger)
    {
        if (!_initialized)
        {
            lock (_lock)
            {
                if (!_initialized)
                {
                    try
                    {
                        logger.LogInformation("Initializing databases...");
                        
                        // Initialize Catalog database
                        if (catalogContext.Database.IsSqlite())
                        {
                            catalogContext.Database.EnsureCreated();
                            logger.LogInformation("Catalog database ensured");
                        }
                        
                        // Initialize Identity database
                        if (identityContext.Database.IsSqlite())
                        {
                            identityContext.Database.EnsureCreated();
                            logger.LogInformation("Identity database ensured");
                        }
                        
                        _initialized = true;
                        logger.LogInformation("Databases initialized successfully");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to initialize databases");
                    }
                }
            }
        }

        await _next(context);
    }
}
