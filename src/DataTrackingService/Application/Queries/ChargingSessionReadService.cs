using DataTrackingService.Data.Mongo.Usage;
using DataTrackingService.Domain.Models.Usage;

namespace DataTrackingService.Application.Queries;

public class ChargingSessionReadService
{
    private readonly ChargingSessionRepository _repository;

    public ChargingSessionReadService(ChargingSessionRepository repository)
    {
        _repository = repository;
    }

    public Task<List<ChargingSessionView>> GetUserSessionsAsync(string userId) =>
        _repository.GetByUserAsync(userId);

    public Task<List<ChargingSessionView>> GetProviderSessionsAsync(string providerId) =>
        _repository.GetByProviderAsync(providerId);

    public Task<List<ChargingSessionView>> GetProviderSessionsByDateRange(string providerId, DateTime start, DateTime end) =>
        _repository.GetByDateRangeAsync(providerId, start, end);
}
