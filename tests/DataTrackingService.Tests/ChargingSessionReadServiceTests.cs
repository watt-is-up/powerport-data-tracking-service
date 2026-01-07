using DataTrackingService.Application.Queries;
using DataTrackingService.Application.Commands;
using DataTrackingService.Data.Mongo.Usage;
using DataTrackingService.Data.Mongo;
using DataTrackingService.Domain.Models.Usage;
using Mongo2Go;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Xunit;

public class ChargingSessionReadServiceTests : IDisposable
{
    private readonly MongoDbRunner _runner;
    private readonly UserChargingSessionQueryService _readService;
    private readonly ChargingSessionWriteService _writeService;
    private readonly IMongoDbContextFactory _factory;
    private readonly ITenantRegistry _tenantRegistry = new TenantRegistry();
    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "MongoDb:ConnectionString", "mongodb://localhost:27017" },
            { "MongoDb:SharedDatabaseName", "shared_database" }
        })
        .Build();

    public ChargingSessionReadServiceTests()
    {
        _runner = MongoDbRunner.Start();
        var client = new MongoClient(_runner.ConnectionString);
        _factory = new MongoDbContextFactory(client, _tenantRegistry, _configuration);

        var repo = new ChargingSessionRepository(_factory);

        var readService = new ChargingSessionReadService(repo);
        var monthlyUsageRepo = new UserMonthlyUsageRepository(_factory);
        _readService = new UserChargingSessionQueryService(readService, monthlyUsageRepo);
        _writeService = new ChargingSessionWriteService(repo);
    }

    [Fact]
    public async Task GetUserSessionsAsync_ShouldReturnSessions_WhenDataExists()
    {
        var session = new ChargingSessionView
        {
            SessionId = Guid.NewGuid().ToString(),
            UserId = "user1",
            SessionStarted = DateTime.UtcNow,
            SessionEnded = DateTime.UtcNow.AddHours(1),
            EnergyKWh = 20,
            Cost = 5.0m
        };

        await _writeService.AddSessionForUserAsync(session);

        var sessions = await _readService._chargingSessionReadService.GetUserSessionsAsync("user1");
        Assert.Single(sessions);
        Assert.Equal(session.EnergyKWh, sessions[0].EnergyKWh);
    }

    [Fact]
    public async Task GetUserSessionsAsync_ShouldReturnEmpty_WhenNoData()
    {
        var sessions = await _readService._chargingSessionReadService.GetUserSessionsAsync("nonexistent");
        Assert.Empty(sessions);
    }

    // [Fact]
    // public async Task GetSessionsByDateRange_ShouldFilterCorrectly()
    // {
    //     var now = DateTime.UtcNow;

    //     await _writeService.AddSessionForProviderAsync(new ChargingSessionView
    //     {
    //         SessionId = Guid.NewGuid().ToString(),
    //         UserId = "user1",
    //         SessionStarted = now.AddDays(-2),
    //         SessionEnded = now.AddDays(-2).AddHours(1),
    //         EnergyKWh = 10,
    //         Cost = 2.0m
    //     });

    //     await _writeService.AddSessionForUserAsync(new ChargingSessionView
    //     {
    //         SessionId = Guid.NewGuid().ToString(),
    //         UserId = "user1",
    //         SessionStarted = now,
    //         SessionEnded = now.AddHours(1),
    //         EnergyKWh = 15,
    //         Cost = 3.5m
    //     });

    //     var sessions = await _readService._chargingSessionReadService.GetProviderSessionsByDateRange("user1", now.AddDays(-1), now.AddDays(1));
    //     Assert.Single(sessions);
    //     Assert.Equal(15, sessions[0].EnergyKWh);
    // }

    public void Dispose() => _runner.Dispose();
}
