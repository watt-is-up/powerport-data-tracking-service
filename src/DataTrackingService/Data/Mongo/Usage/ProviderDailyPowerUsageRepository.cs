using DataTrackingService.Domain.Models.Providers;
using MongoDB.Driver;

namespace DataTrackingService.Data.Mongo.Usage;

public class ProviderDailyPowerUsageRepository
{
    private readonly IMongoDbContextFactory _factory;
    public ProviderDailyPowerUsageRepository(IMongoDbContextFactory factory)
    {
        _factory = factory;
    }

    public async Task UpsertAsync(
        string providerId,
        DateTime date,
        decimal energyDelta)
    {
        var day = date.Date;
        var id = $"{providerId}-{day:yyyy-MM-dd}";

        var update = Builders<ProviderDailyPowerUsageView>.Update
            .Inc(x => x.TotalEnergyKWh, energyDelta)
            .Inc(x => x.SessionsCount, 1)
            .Set(x => x.UpdatedAt, DateTime.UtcNow)
            .SetOnInsert(x => x.Id, id)
            .SetOnInsert(x => x.ProviderId, providerId)
            .SetOnInsert(x => x.Date, day);

        var context = _factory.GetTenantContext(providerId);
        await context.ProviderDailyPowerUsage.UpdateOneAsync(
            x => x.Id == id,
            update,
            new UpdateOptions { IsUpsert = true });
    }

    public Task<List<ProviderDailyPowerUsageView>> GetAsync(
        string providerId,
        DateTime from,
        DateTime to)
    {
        var context = _factory.GetTenantContext(providerId);
        return context.ProviderDailyPowerUsage.Find(x =>
                x.ProviderId == providerId &&
                x.Date >= from.Date &&
                x.Date <= to.Date)
            .SortBy(x => x.Date)
            .ToListAsync();
    }
}
