using DataTrackingService.Data.Mongo.Usage;
using DataTrackingService.Domain.Models.Usage;
using DataTrackingService.Messaging.Events.Consuming;

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

    public Task SaveSessionBillingForUserAsync(ChargingSessionFinalized ChargingFinalizedEvent)
    {
        var session = new ChargingSessionView
        {
            SessionId = ChargingFinalizedEvent.SessionId,
            UserId = ChargingFinalizedEvent.UserId,
            SessionStarted = ChargingFinalizedEvent.SessionStarted,
            SessionEnded = ChargingFinalizedEvent.SessionEnded,
            EnergyKWh = ChargingFinalizedEvent.TotalEnergyKwh,
            Cost = ChargingFinalizedEvent.Amount
        };
        return _repository.InsertForUserAsync(session);
    }

    public Task SaveSessionBillingForTenantAsync(ChargingSessionFinalized ChargingFinalizedEvent)
    {

        var session = new ChargingSessionView
        {
            SessionId = ChargingFinalizedEvent.SessionId,
            UserId = ChargingFinalizedEvent.UserId,
            SessionStarted = ChargingFinalizedEvent.SessionStarted,
            SessionEnded = ChargingFinalizedEvent.SessionEnded,
            EnergyKWh = ChargingFinalizedEvent.TotalEnergyKwh,
            Cost = ChargingFinalizedEvent.Amount
        };
        return _repository.InsertForProviderAsync(ChargingFinalizedEvent.ProviderId, session);
    }
}
