using DataTrackingService.Domain.Models.Spreadsheets;
using System.Text.Json;

namespace DataTrackingService.Application.Validation;

public static class SpreadsheetRowValidator
{
    public static void Validate(
        UserSpreadsheet spreadsheet,
        Dictionary<string, object?> values)
    {
        foreach (var column in spreadsheet.Columns)
        {
            values.TryGetValue(column.ColumnId, out var value);

            if (column.Required && value == null)
                throw new InvalidOperationException(
                    $"Column '{column.Name}' is required");

            if (value == null)
                continue;

            if (!IsValidType(column.Type, value))
                throw new InvalidOperationException(
                    $"Invalid value for column '{column.Name}'");
        }
    }

    private static bool IsValidType(
        SpreadsheetColumnType type,
        object value)
    {
        if (value is null)
            return false;

        return type switch
        {
            SpreadsheetColumnType.String =>
                value is string ||
                (value is JsonElement j && j.ValueKind == JsonValueKind.String),

            SpreadsheetColumnType.Number =>
                value is int ||
                value is long ||
                value is double ||
                value is decimal ||
                (value is JsonElement j && j.ValueKind == JsonValueKind.Number),

            SpreadsheetColumnType.Date =>
                value is DateTime ||
                (value is string s && DateTime.TryParse(s, out _)) ||
                (value is JsonElement j &&
                    j.ValueKind == JsonValueKind.String &&
                    DateTime.TryParse(j.GetString(), out _)),

            _ => false
        };
    }
    
}
