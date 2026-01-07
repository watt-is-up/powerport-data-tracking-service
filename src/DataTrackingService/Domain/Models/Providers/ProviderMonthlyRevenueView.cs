using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataTrackingService.Domain.Models.Providers;

public class ProviderMonthlyRevenueView
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = null!; // providerId-yyyy-MM

    public string ProviderId { get; set; } = null!;
    public int Year { get; set; }
    public int Month { get; set; }

    public decimal TotalRevenue { get; set; }
    public int PaymentsCount { get; set; }

    public DateTime UpdatedAt { get; set; }
}
