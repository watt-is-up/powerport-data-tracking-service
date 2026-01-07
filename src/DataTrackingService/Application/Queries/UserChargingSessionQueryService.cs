using DataTrackingService.Data.Mongo.Usage;
using DataTrackingService.Domain.Models.Usage;

namespace DataTrackingService.Application.Queries;

public class UserChargingSessionQueryService
{
    public readonly ChargingSessionReadService _chargingSessionReadService;
    private readonly UserMonthlyUsageRepository _repository;

    public UserChargingSessionQueryService(
        ChargingSessionReadService chargingSessionReadService,
        UserMonthlyUsageRepository repository)
    {
        _chargingSessionReadService = chargingSessionReadService;
        _repository = repository;
    }
    
    public async Task<List<UserMonthlyUsageView>> GetMonthlyUsage(string userId)
    {
        return await _repository.GetByUserAsync(userId);
    }

}