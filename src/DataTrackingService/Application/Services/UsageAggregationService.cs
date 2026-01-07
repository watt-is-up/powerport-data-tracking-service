using DataTrackingService.Data.Mongo.Usage;

namespace DataTrackingService.Application.Services;

public class UsageAggregationService
{
    private readonly UserMonthlyUsageRepository _repository;

    public UsageAggregationService(UserMonthlyUsageRepository repository)
    {
        _repository = repository;
    }

    public Task ApplyChargingSessionAsync(
        string userId,
        DateTime sessionEnd,
        decimal energyKWh,
        decimal cost)
    {
        return _repository.UpsertAsync(
            userId,
            sessionEnd.Year,
            sessionEnd.Month,
            energyKWh,
            cost);
    }
}
