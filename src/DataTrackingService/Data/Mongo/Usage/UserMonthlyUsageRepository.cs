using DataTrackingService.Domain.Models.Usage;
using MongoDB.Driver;

namespace DataTrackingService.Data.Mongo.Usage;

public class UserMonthlyUsageRepository
{
    private readonly IMongoDbContextFactory _factory;

    public UserMonthlyUsageRepository(IMongoDbContextFactory factory)
    {
        _factory = factory;
    }

    public async Task UpsertAsync(
        string userId,
        int year,
        int month,
        decimal energyDelta,
        decimal costDelta)
    {
        var id = $"{userId}-{year}-{month:D2}";

        var update = Builders<UserMonthlyUsageView>.Update
            .Inc(x => x.TotalEnergyKWh, energyDelta)
            .Inc(x => x.TotalCost, costDelta)
            .Inc(x => x.SessionsCount, 1)
            .Set(x => x.UpdatedAt, DateTime.UtcNow)
            .SetOnInsert(x => x.Id, id)
            .SetOnInsert(x => x.UserId, userId)
            .SetOnInsert(x => x.Year, year)
            .SetOnInsert(x => x.Month, month);

        var context = _factory.GetUserContext();
        await context.UserMonthlyUsage.UpdateOneAsync(
            x => x.Id == id,
            update,
            new UpdateOptions { IsUpsert = true });
    }

    public Task<List<UserMonthlyUsageView>> GetByUserAsync(string userId)
    {
        var context = _factory.GetUserContext();
        return context.UserMonthlyUsage
            .Find(x => x.UserId == userId)
            .SortBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();
    }
}
