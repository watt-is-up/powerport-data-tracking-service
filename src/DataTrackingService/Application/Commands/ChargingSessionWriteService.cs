using DataTrackingService.Data.Mongo.Usage;
using DataTrackingService.Domain.Models.Usage;

namespace DataTrackingService.Application.Commands;

public class ChargingSessionWriteService
{
    private readonly ChargingSessionRepository _repository;

    public ChargingSessionWriteService(ChargingSessionRepository repository)
    {
        _repository = repository;
    }

    public Task AddSessionForUserAsync(ChargingSessionView session) =>
        _repository.InsertForUserAsync(session);

    public Task AddSessionForProviderAsync(string providerId, ChargingSessionView session) =>
        _repository.InsertForProviderAsync(providerId, session);
}
