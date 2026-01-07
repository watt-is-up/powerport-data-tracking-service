using DataTrackingService.Application.Validation;
using DataTrackingService.Data.Mongo.Spreadsheets;
using DataTrackingService.Domain.Models.Spreadsheets;
using System.Text.Json;

namespace DataTrackingService.Application.Commands;

public class UserSpreadsheetWriteService
{
    private readonly UserSpreadsheetRepository _sheetRepo;
    private readonly UserSpreadsheetRowRepository _rowRepo;

    public UserSpreadsheetWriteService(
        UserSpreadsheetRepository sheetRepo,
        UserSpreadsheetRowRepository rowRepo)
    {
        _sheetRepo = sheetRepo;
        _rowRepo = rowRepo;
    }

    public async Task<UserSpreadsheet> CreateSpreadsheetAsync(
        string userId,
        string name)
    {
        var sheet = new UserSpreadsheet
        {
            UserId = userId,
            Name = name,
            Columns = new List<SpreadsheetColumn>()
        };

        await _sheetRepo.InsertAsync(sheet);
        return sheet;
    }

    public async Task<SpreadsheetColumn> AddColumnAsync(
        string spreadsheetId,
        string userId,
        SpreadsheetColumn column)
    {
        var sheet = await _sheetRepo.GetAsync(spreadsheetId, userId);
        sheet.Columns.Add(column);
        await _sheetRepo.ReplaceAsync(sheet);

        return column;
    }

    public async Task AddRowAsync(
        string spreadsheetId,
        string userId,
        Dictionary<string, object?> values)
    {
        var sheet = await _sheetRepo.GetAsync(spreadsheetId, userId);

        SpreadsheetRowValidator.Validate(sheet, values);
        values = NormalizeValues(values);

        var row = new UserSpreadsheetRow
        {
            SpreadsheetId = spreadsheetId,
            UserId = userId,
            Values = values
        };

        await _rowRepo.InsertAsync(row);
    }

    private static Dictionary<string, object?> NormalizeValues(
        Dictionary<string, object?> values)
    {
        var result = new Dictionary<string, object?>();

        foreach (var (key, value) in values)
        {
            if (value is not JsonElement json)
            {
                result[key] = value;
                continue;
            }

            result[key] = json.ValueKind switch
            {
                JsonValueKind.String =>
                    json.TryGetDateTime(out var dt)
                        ? dt
                        : json.GetString(),

                JsonValueKind.Number =>
                    json.TryGetInt64(out var l)
                        ? l
                        : json.GetDouble(),

                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,

                _ => throw new InvalidOperationException(
                    $"Unsupported JSON value for column '{key}'")
            };
        }

        return result;
    }
}
