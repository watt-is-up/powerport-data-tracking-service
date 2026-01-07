using MongoDB.Driver;

using DataTrackingService.Domain.Models.Providers;

namespace DataTrackingService.Data.Mongo;

public class MongoDbContextFactory : IMongoDbContextFactory
{
    private readonly IMongoClient _client;
    private readonly ITenantRegistry _tenantRegistry;
    private readonly string _sharedDatabaseName;

    public MongoDbContextFactory(
        IMongoClient client,
        ITenantRegistry tenantRegistry,
        IConfiguration config)
    {
        _client = client;
        _tenantRegistry = tenantRegistry;
        
        var settings = config.GetSection("MongoDb");
        _sharedDatabaseName = settings["SharedDatabaseName"] 
            ?? throw new ArgumentNullException("MongoDb:SharedDatabaseName is missing");
    }

    public UserMongoDbContext GetUserContext()
    {   
        var db = _client.GetDatabase(_sharedDatabaseName);
        return new UserMongoDbContext(db);
    }

    public TenantMongoDbContext GetTenantContext(string providerId)
    {
        var tenant = _tenantRegistry.Get(providerId);

        var dbName = tenant.Mode == TenantMode.Isolated
            ? tenant.DatabaseName
            : "shared_providers";

        var db = _client.GetDatabase(dbName);
        return new TenantMongoDbContext(db);
    }
}
