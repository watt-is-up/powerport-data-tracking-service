// In your test project: TestMongoDbContextFactory.cs
using DataTrackingService.Data.Mongo;
using DataTrackingService.Data.Mongo.Multitenancy;
using DataTrackingService.Domain.Models.Providers;
using MongoDB.Driver;

public class TestMongoDbContextFactory : IMongoDbContextFactory
{
    private readonly MongoClient _client;
    private readonly ITenantRegistry _tenantRegistry;
    private readonly string _sharedDatabase;

    public TestMongoDbContextFactory(
        MongoClient client,
        ITenantRegistry tenantRegistry,
        string sharedDatabase = "data_tracking_shared")
    {
        _client = client;
        _tenantRegistry = tenantRegistry;
        _sharedDatabase = sharedDatabase;
    }

    public UserMongoDbContext GetUserContext()
    {
        var db = _client.GetDatabase(_sharedDatabase);
        return new UserMongoDbContext(db);
    }

    public TenantMongoDbContext GetTenantContext(string providerId)
    {
        var tenant = _tenantRegistry.Get(providerId);
        
        if (tenant == null)
            throw new InvalidOperationException($"Tenant '{providerId}' not found");
            
        if (tenant.Mode != TenantMode.Isolated)
            throw new InvalidOperationException("Tenant is not isolated");

        var db = _client.GetDatabase(tenant.DatabaseConnection.DatabaseName);
        return new TenantMongoDbContext(db);
    }
}