using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataTrackingService.Domain.Models.Usage;

public class UserMonthlyUsageView
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = null!; // userId-yyyy-MM

    public string UserId { get; set; } = null!;
    public int Year { get; set; }
    public int Month { get; set; }

    public decimal TotalEnergyKWh { get; set; }
    public decimal TotalCost { get; set; }
    public int SessionsCount { get; set; }

    public DateTime UpdatedAt { get; set; }
}
