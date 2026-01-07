using DataTrackingService.Domain.Models.Spreadsheets;
using MongoDB.Driver;

namespace DataTrackingService.Data.Mongo.Spreadsheets;

public class UserSpreadsheetRepository
{
    private readonly IMongoDbContextFactory _factory;

    public UserSpreadsheetRepository(IMongoDbContextFactory factory)
    {
        _factory = factory;
    }

    public Task InsertAsync(UserSpreadsheet spreadsheet) =>
        _factory.GetUserContext().UserSpreadsheets.InsertOneAsync(spreadsheet);

    public async Task<UserSpreadsheet> GetAsync(string spreadsheetId, string userId)
    {
        var context = _factory.GetUserContext();
        var sheet = await context.UserSpreadsheets.Find(x =>
            x.Id == spreadsheetId && x.UserId == userId).FirstOrDefaultAsync();

        if (sheet == null)
            throw new KeyNotFoundException("Spreadsheet not found");

        return sheet;
    }

    public Task<List<UserSpreadsheet>> GetByUserAsync(string userId) =>
        _factory.GetUserContext().UserSpreadsheets.Find(x => x.UserId == userId).ToListAsync();

    public Task ReplaceAsync(UserSpreadsheet sheet) =>
        _factory.GetUserContext().UserSpreadsheets.ReplaceOneAsync(x => x.Id == sheet.Id, sheet);
}
