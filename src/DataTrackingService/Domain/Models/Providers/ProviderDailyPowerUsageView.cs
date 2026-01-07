using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataTrackingService.Domain.Models.Providers;

public class ProviderDailyPowerUsageView
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = null!; // providerId-yyyy-MM-dd

    public string ProviderId { get; set; } = null!;
    public DateTime Date { get; set; }

    public decimal TotalEnergyKWh { get; set; }
    public int SessionsCount { get; set; }

    public DateTime UpdatedAt { get; set; }
}
