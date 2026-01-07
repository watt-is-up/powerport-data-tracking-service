using DataTrackingService.Data.Mongo.Usage;

namespace DataTrackingService.Application.Services;

public class ProviderAggregationService
{
    private readonly ProviderMonthlyRevenueRepository _revenueRepo;
    private readonly ProviderDailyPowerUsageRepository _powerRepo;

    public ProviderAggregationService(
        ProviderMonthlyRevenueRepository revenueRepo,
        ProviderDailyPowerUsageRepository powerRepo)
    {
        _revenueRepo = revenueRepo;
        _powerRepo = powerRepo;
    }

    public Task ApplyPaymentAsync(
        string providerId,
        DateTime paidAt,
        decimal amount)
    {
        return _revenueRepo.UpsertAsync(
            providerId,
            paidAt.Year,
            paidAt.Month,
            amount);
    }

    public Task ApplyChargingSessionAsync(
        string providerId,
        DateTime sessionEnd,
        decimal energyKWh)
    {
        return _powerRepo.UpsertAsync(
            providerId,
            sessionEnd,
            energyKWh);
    }
}
