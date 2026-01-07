using MongoDB.Driver;
using DataTrackingService.Domain.Models.Spreadsheets;
using DataTrackingService.Domain.Models.Usage;
using DataTrackingService.Domain.Models.Providers;

namespace DataTrackingService.Data.Mongo;

public class TenantMongoDbContext
{
    private readonly IMongoDatabase _database;

    public TenantMongoDbContext(IMongoDatabase database)
    {
        _database = database;
    }

    public IMongoCollection<ChargingSessionView> ChargingSessions =>
        _database.GetCollection<ChargingSessionView>("charging_sessions");

    public IMongoCollection<ProviderDailyPowerUsageView> ProviderDailyPowerUsage =>
        _database.GetCollection<ProviderDailyPowerUsageView>("provider_daily_power_usage");

    public IMongoCollection<ProviderMonthlyRevenueView> ProviderMonthlyRevenue =>
        _database.GetCollection<ProviderMonthlyRevenueView>("provider_monthly_revenue");    
}
