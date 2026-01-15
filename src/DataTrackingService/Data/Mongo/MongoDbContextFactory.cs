using MongoDB.Driver;
using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

using DataTrackingService.Domain.Models.Providers;
using DataTrackingService.Data.Mongo.Multitenancy;

namespace DataTrackingService.Data.Mongo;
public interface IMongoDbContextFactory
{
    UserMongoDbContext GetUserContext();
    TenantMongoDbContext GetTenantContext(string providerId);
}

public sealed class MongoDbContextFactory: IMongoDbContextFactory
{
    private readonly ILogger<MongoDbContextFactory> _logger;
    private readonly ITenantRegistry _tenantRegistry;
    private readonly string _host;
    private readonly int _port;
    private readonly string _sharedDatabaseName;
    private readonly MongoClient _sharedClient;
    private readonly ConcurrentDictionary<string, MongoClient> _tenantClients = new();

    public MongoDbContextFactory(
        ITenantRegistry tenantRegistry,
        IOptions<MongoOptions> options,
        ILogger<MongoDbContextFactory> logger)
    {
        _tenantRegistry = tenantRegistry;
        _logger = logger;

        _host = options.Value.Host;
        _port = options.Value.Port;

        // Shared DB client (single user)
        var sharedTenant = _tenantRegistry.GetSharedTenant();
        _sharedDatabaseName = sharedTenant.DatabaseConnection.DatabaseName;
        _sharedClient = CreateClient(
            _host,
            _port,
            sharedTenant.DatabaseConnection.User,
            sharedTenant.DatabaseConnection.Password,
            authDb: _sharedDatabaseName  // <- use the DB the user exists in
        );
    }

    private MongoClient CreateClient(
        string host,
        int port,
        string username,
        string password,
        string authDb)
    {
        var url = new MongoUrlBuilder
        {
            Server = new MongoServerAddress(host, port),
            Username = username,
            Password = password,
            AuthenticationSource = authDb
        };
        
        _logger.LogInformation("Creating mongo Client with url {url}", url.ToMongoUrl());
        return new MongoClient(url.ToMongoUrl());
    }

    public UserMongoDbContext GetUserContext()
    {
        var db = _sharedClient.GetDatabase(_sharedDatabaseName);
        return new UserMongoDbContext(db);
    }

    public TenantMongoDbContext GetTenantContext(string providerId)
    {
        var tenant = _tenantRegistry.Get(providerId);

        if (tenant.Mode != TenantMode.Isolated)
            throw new InvalidOperationException("Tenant is not isolated");

        var client = _tenantClients.GetOrAdd(
            tenant.Id,
            _ => CreateClient(
                _host,
                _port,
                tenant.DatabaseConnection.User,
                tenant.DatabaseConnection.Password,
                tenant.DatabaseConnection.DatabaseName)
        );

        var db = client.GetDatabase(tenant.DatabaseConnection.DatabaseName);
        return new TenantMongoDbContext(db);
    }
}

public sealed class MongoOptions
{
    public string Host { get; init; } = default!;
    public int Port { get; init; }
}
