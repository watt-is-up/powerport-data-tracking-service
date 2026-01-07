using DataTrackingService.Data.Mongo;
using DataTrackingService.Domain.Models.Spreadsheets;
using MongoDB.Driver;


public class MongoIndexInitializer
{

    private readonly IMongoDbContextFactory _contextFactory;

    public MongoIndexInitializer(IMongoDbContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }


    public async Task EnsureIndexesAsync()
    {
        var _context = _contextFactory.GetUserContext();
        await _context.UserSpreadsheets.Indexes.CreateOneAsync(
            new CreateIndexModel<UserSpreadsheet>(
                Builders<UserSpreadsheet>.IndexKeys
                    .Ascending(x => x.UserId)
                    .Ascending(x => x.Name),
                new CreateIndexOptions { Unique = true }
            )
        );

        await _context.UserSpreadsheetRows.Indexes.CreateOneAsync(
            new CreateIndexModel<UserSpreadsheetRow>(
                Builders<UserSpreadsheetRow>.IndexKeys
                    .Ascending(x => x.SpreadsheetId)
                    .Ascending(x => x.UserId)
            )
        );
    }
}
