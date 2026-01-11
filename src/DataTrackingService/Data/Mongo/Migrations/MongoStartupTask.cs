using Microsoft.Extensions.Hosting;

using DataTrackingService.Data.Mongo.Multitenancy;

namespace DataTrackingService.Data.Mongo.Migrations;
public class MongoStartupTask : IHostedService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<MongoStartupTask> _logger;

    public MongoStartupTask(IServiceProvider provider, ILogger<MongoStartupTask> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _provider.CreateScope();

        // -------------------- Apply Migrations --------------------  
        var services = scope.ServiceProvider;
        var tenantRegistry = services.GetRequiredService<ITenantRegistry>();
        var contextFactory = services.GetRequiredService<IMongoDbContextFactory>();
        var runner = services.GetRequiredService<MongoMigrationRunner>();

        try
        {
            // Shared DB
            var sharedContext = contextFactory.GetUserContext();
            await runner.RunAsync(sharedContext.Database);

            // Tenant DBs
            foreach (var tenant in tenantRegistry.GetTenants())
            {
                var tenantContext = contextFactory.GetTenantContext(tenant.Id);
                await runner.RunAsync(tenantContext.Database);
            }
            _logger.LogInformation("Mongo migrations run successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating MongoDB databases.");
            throw;
        }

        // -------------------- Apply Indexes --------------------  
        var initializer = scope.ServiceProvider.GetRequiredService<MongoIndexInitializer>();
        try
        {
            await initializer.EnsureUserIndexesAsync();

            foreach (var tenant in tenantRegistry.GetTenants())
            {
                await initializer.EnsureTenantIndexesAsync(tenant);
            }
            _logger.LogInformation("Mongo indexes initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize MongoDB indexes");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
