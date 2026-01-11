using DataTrackingService.Domain.Models.Usage;
using MongoDB.Driver;

namespace DataTrackingService.Data.Mongo.Usage;
public class ChargingSessionRepository
{
    private readonly IMongoDbContextFactory _factory;

    public ChargingSessionRepository(IMongoDbContextFactory factory)
    {
        _factory = factory;
    }

    // For user sessions
    public Task InsertForUserAsync(ChargingSessionView session)
    {
        var context = _factory.GetUserContext();
        return context.ChargingSessions.InsertOneAsync(session);
    }

    // For provider sessions
    public Task InsertForProviderAsync(string providerId, ChargingSessionView session)
    {
        var context = _factory.GetTenantContext(providerId);
        return context.ChargingSessions.InsertOneAsync(session);
    }

    public Task<List<ChargingSessionView>> GetByUserAsync(string userId)
    {
        var context = _factory.GetUserContext();
        return context.ChargingSessions
            .Find(x => x.UserId == userId)
            .ToListAsync();
    }

    public Task<List<ChargingSessionView>> GetByProviderAsync(string providerId)
    {
        var context = _factory.GetTenantContext(providerId);
        return context.ChargingSessions
            .Find(_ => true)
            .ToListAsync();
    }

    public Task<List<ChargingSessionView>> GetByDateRangeAsync(
        string providerId,
        DateTime start,
        DateTime end)
    {
        var context = _factory.GetTenantContext(providerId);
        return context.ChargingSessions
            .Find(x => x.SessionStarted >= start && x.SessionEnded <= end)
            .ToListAsync();
    }
}