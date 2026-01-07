using DataTrackingService.Domain.Models.Providers;
using MongoDB.Driver;

namespace DataTrackingService.Data.Mongo.Usage;

public class ProviderMonthlyRevenueRepository
{
    private readonly IMongoDbContextFactory _factory;

    public ProviderMonthlyRevenueRepository(IMongoDbContextFactory factory)
    {
        _factory = factory;
    }


    public async Task UpsertAsync(
        string providerId,
        int year,
        int month,
        decimal revenueDelta)
    {
        var id = $"{providerId}-{year}-{month:D2}";

        var update = Builders<ProviderMonthlyRevenueView>.Update
            .Inc(x => x.TotalRevenue, revenueDelta)
            .Inc(x => x.PaymentsCount, 1)
            .Set(x => x.UpdatedAt, DateTime.UtcNow)
            .SetOnInsert(x => x.Id, id)
            .SetOnInsert(x => x.ProviderId, providerId)
            .SetOnInsert(x => x.Year, year)
            .SetOnInsert(x => x.Month, month);

        var context = _factory.GetTenantContext(providerId);
        await context.ProviderMonthlyRevenue.UpdateOneAsync(
            x => x.Id == id,
            update,
            new UpdateOptions { IsUpsert = true });
    }

    public Task<List<ProviderMonthlyRevenueView>> GetByProviderAsync(string providerId)
    {
        var context = _factory.GetTenantContext(providerId);
        return context.ProviderMonthlyRevenue
            .Find(x => x.ProviderId == providerId)
            .SortBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();
    }
    
}
