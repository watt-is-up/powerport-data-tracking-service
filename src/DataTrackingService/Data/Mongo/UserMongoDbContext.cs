using MongoDB.Driver;
using DataTrackingService.Domain.Models.Spreadsheets;
using DataTrackingService.Domain.Models.Usage;

namespace DataTrackingService.Data.Mongo;

public class UserMongoDbContext
{
    private readonly IMongoDatabase _database;

    public UserMongoDbContext(IMongoDatabase database)
    {
        _database = database;
    }

    public IMongoCollection<UserSpreadsheet> UserSpreadsheets =>
        _database.GetCollection<UserSpreadsheet>("user_spreadsheets");

    public IMongoCollection<UserSpreadsheetRow> UserSpreadsheetRows =>
        _database.GetCollection<UserSpreadsheetRow>("user_spreadsheet_rows");
    public IMongoCollection<ChargingSessionView> ChargingSessions =>
        _database.GetCollection<ChargingSessionView>("charging_sessions");

    public IMongoCollection<UserMonthlyUsageView> UserMonthlyUsage =>
        _database.GetCollection<UserMonthlyUsageView>("user_monthly_usage");
}
