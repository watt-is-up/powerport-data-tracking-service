using DataTrackingService.Data.Mongo.Usage;
using DataTrackingService.Domain.Models.Providers;

namespace DataTrackingService.Application.Queries;

public class ProviderChargingSessionQueryService
{
    private readonly ChargingSessionReadService _chargingSessionReadService;
    private readonly ProviderMonthlyRevenueRepository _revenueRepo;
    private readonly ProviderDailyPowerUsageRepository _powerRepo;
    
    public ProviderChargingSessionQueryService(
        ChargingSessionReadService chargingSessionReadService,
        ProviderMonthlyRevenueRepository revenueRepo,
        ProviderDailyPowerUsageRepository powerRepo)
    {
        _chargingSessionReadService = chargingSessionReadService;
        _revenueRepo = revenueRepo;
        _powerRepo = powerRepo;
    }

    public async Task<List<ProviderMonthlyRevenueView>> GetMonthlyRevenue(string providerId)
    {
        return await _revenueRepo.GetByProviderAsync(providerId);
    }

    public async Task<List<ProviderDailyPowerUsageView>> GetPowerUsage(
        string providerId,
        DateTime from,
        DateTime to)
    {
        return await _powerRepo.GetAsync(providerId, from, to);
    }

    public Task<List<Domain.Models.Usage.ChargingSessionView>> GetProviderSessionsAsync(string providerId) =>
        _chargingSessionReadService.GetProviderSessionsAsync(providerId);
}