using MongoDB.Driver;
using MongoDB.Bson;

using DataTrackingService.Domain.Models.Spreadsheets;
using DataTrackingService.Application.Dtos;


namespace DataTrackingService.Data.Mongo.Spreadsheets;

public class UserSpreadsheetRowRepository
{
    private readonly IMongoCollection<UserSpreadsheetRow> _collection;

    public UserSpreadsheetRowRepository(IMongoDbContextFactory contextFactory)
    {
        var context = contextFactory.GetUserContext();
        _collection = context.UserSpreadsheetRows;
    
        // Mandatory index for performance
        _collection.Indexes.CreateOne(
            new CreateIndexModel<UserSpreadsheetRow>(
                Builders<UserSpreadsheetRow>.IndexKeys
                    .Ascending(x => x.SpreadsheetId)
                    .Ascending(x => x.UserId)));
    }

    public Task InsertAsync(UserSpreadsheetRow row) =>
        _collection.InsertOneAsync(row);

    public Task<List<UserSpreadsheetRow>> GetBySpreadsheetAsync(
        string spreadsheetId,
        string userId)
    {
        return _collection.Find(x =>
            x.SpreadsheetId == spreadsheetId &&
            x.UserId == userId).ToListAsync();
    }

    public async Task<List<MonthlySum>> SumByMonthAsync(
        string spreadsheetId,
        string userId,
        string columnName)
    {
        var pipeline = new[]
        {
            new BsonDocument("$match", new BsonDocument
            {
                { "SpreadsheetId", spreadsheetId },
                { "UserId", userId }
            }),
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", new BsonDocument("$month", "$CreatedAt") },
                { "total", new BsonDocument("$sum", $"$Values.{columnName}") }
            }),
            new BsonDocument("$sort", new BsonDocument("_id", 1))
        };

        var docs = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();

        return docs.Select(d => new MonthlySum
        {
            Month = d["_id"].AsInt32,
            Total = d["total"].ToDecimal()
        }).ToList();
    }
}
