using DataTrackingService.Data.Mongo.Spreadsheets;
using DataTrackingService.Domain.Models.Spreadsheets;

namespace DataTrackingService.Application.Queries;

public class UserSpreadsheetReadService
{
    private readonly UserSpreadsheetRepository _sheetRepo;
    private readonly UserSpreadsheetRowRepository _rowRepo;

    public UserSpreadsheetReadService(
        UserSpreadsheetRepository sheetRepo,
        UserSpreadsheetRowRepository rowRepo)
    {
        _sheetRepo = sheetRepo;
        _rowRepo = rowRepo;
    }

    public async Task<UserSpreadsheet> GetSpreadsheetAsync(
        string spreadsheetId,
        string userId)
    {
        return await _sheetRepo.GetAsync(spreadsheetId, userId);
    }

    public async Task<List<UserSpreadsheetRow>> GetRowsAsync(
        string spreadsheetId,
        string userId)
    {
        return await _rowRepo.GetBySpreadsheetAsync(spreadsheetId, userId);
    }

    public async Task<Dictionary<string, decimal>> SumByMonthAsync(
        string spreadsheetId,
        string userId,
        string column)
    {
        var monthlySums = await _rowRepo.SumByMonthAsync(
            spreadsheetId,
            userId,
            column);

        return monthlySums.ToDictionary(
            x => x.Month.ToString(),
            x => x.Total);
    }

}
