using FluentAssertions;
using DataTrackingService.Application.Commands;
using DataTrackingService.Messaging.Events.Consuming;
using DataTrackingService.Data.Mongo.Usage;
using DataTrackingService.Data.Mongo;
using DataTrackingService.Infrastructure.Multitenancy;
using DataTrackingService.Data.Mongo.Multitenancy;
using Mongo2Go;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Moq;

public class ChargingSessionWriteServiceTests : IDisposable
{
    private readonly MongoDbRunner _runner;
    private readonly IMongoDbContextFactory _factory;
    private readonly ITenantRegistry _tenantRegistry;

    // ChargingSessionWriteServiceTests.cs
    public ChargingSessionWriteServiceTests()
    {
        _runner = MongoDbRunner.Start();
        _tenantRegistry = new TenantRegistry(TenantBootstrap.GetMockTenants());

        var client = new MongoClient(_runner.ConnectionString);
        _factory = new TestMongoDbContextFactory(client, _tenantRegistry);
    }

    [Fact]
    public async Task SaveSessionBillingForTenantAsync_FinalizedEvent_PersistsUsage()
    {
        var repo = new ChargingSessionRepository(_factory);
        var service = new ChargingSessionWriteService(repo);

        // Use actual tenant ID from TenantBootstrap
        var tenantId = TenantBootstrap.GetMockTenants().First().Id;

        var evt = new ChargingSessionFinalized
        {
            SessionId = Guid.NewGuid().ToString(),
            UserId = "user-1",
            ProviderId = tenantId,  // Fix: use valid tenant ID
            TotalEnergyKwh = 15,
            Amount = 6.50m,
            SessionStarted = DateTime.UtcNow.AddHours(-1),
            SessionEnded = DateTime.UtcNow
        };

        await service.SaveSessionBillingForTenantAsync(evt);

        var db = _factory.GetTenantContext(evt.ProviderId);
        var collection = db.ChargingSessions;
        var inserted = await collection.Find(x => x.SessionId == evt.SessionId).FirstOrDefaultAsync();

        inserted.Should().NotBeNull();
        inserted.EnergyKWh.Should().Be(evt.TotalEnergyKwh);
        inserted.UserId.Should().Be(evt.UserId);
    }

    public void Dispose()
    {
        _runner.Dispose();
    }
}