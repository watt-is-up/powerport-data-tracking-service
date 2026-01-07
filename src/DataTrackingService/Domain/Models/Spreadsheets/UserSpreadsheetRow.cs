using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataTrackingService.Domain.Models.Spreadsheets;

public class UserSpreadsheetRow
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string SpreadsheetId { get; set; } = null!;
    public string UserId { get; set; } = null!;

    // ColumnId -> Value
    public Dictionary<string, object?> Values { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
