using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataTrackingService.Domain.Models.Usage;

public class ChargingSessionView
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string SessionId { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public string StationId { get; set; } = null!;

    public DateTime SessionStarted { get; set; }
    public DateTime SessionEnded { get; set; }

    public decimal EnergyKWh { get; set; }
    public decimal Cost { get; set; }
}
