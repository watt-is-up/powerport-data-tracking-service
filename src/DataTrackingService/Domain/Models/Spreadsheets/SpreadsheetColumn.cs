using MongoDB.Bson;

namespace DataTrackingService.Domain.Models.Spreadsheets;

public class SpreadsheetColumn
{
    public string ColumnId { get; set; } = ObjectId.GenerateNewId().ToString(); // GUID string
    public string Name { get; set; } = null!;
    public SpreadsheetColumnType Type { get; set; }
    public bool Required { get; set; }

}
