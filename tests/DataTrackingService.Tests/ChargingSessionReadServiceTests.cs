using DataTrackingService.Application.Queries;
using DataTrackingService.Application.Commands;
using DataTrackingService.Data.Mongo.Usage;
using DataTrackingService.Data.Mongo;
using DataTrackingService.Domain.Models.Usage;
using DataTrackingService.Infrastructure.Multitenancy;
using DataTrackingService.Data.Mongo.Multitenancy;
using Mongo2Go;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Moq;

public class ChargingSessionReadServiceTests : IDisposable
{
    private readonly MongoDbRunner _runner;
    private readonly UserChargingSessionQueryService _readService;
    private readonly ChargingSessionWriteService _writeService;
    private readonly IMongoDbContextFactory _factory;
    private readonly ITenantRegistry _tenantRegistry;

    // ChargingSessionReadServiceTests.cs
    public ChargingSessionReadServiceTests()
    {
        _runner = MongoDbRunner.Start();
        _tenantRegistry = new TenantRegistry(TenantBootstrap.GetMockTenants());

        var client = new MongoClient(_runner.ConnectionString);
        _factory = new TestMongoDbContextFactory(client, _tenantRegistry);

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

    public void Dispose() => _runner.Dispose();
}