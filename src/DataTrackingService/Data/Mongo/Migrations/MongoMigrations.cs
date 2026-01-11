using MongoDB.Driver;
using MongoDB.Bson;

namespace DataTrackingService.Data.Mongo.Migrations;
public interface IMongoMigration
{
    string Id { get; }
    Task ApplyAsync(IMongoDatabase db, CancellationToken ct = default);
}

public sealed class AddPaymentStatusMigration : IMongoMigration
{
    public string Id => "2026-01-10-add-payment-status";

    public async Task ApplyAsync(IMongoDatabase db, CancellationToken ct = default)
    {
        var payments = db.GetCollection<BsonDocument>("payments");

        var filter = Builders<BsonDocument>.Filter.Exists("status", false);
        var update = Builders<BsonDocument>.Update.Set("status", "pending");

        await payments.UpdateManyAsync(filter, update, cancellationToken: ct);
    }
}

public sealed class RenameEnergyFieldMigration : IMongoMigration
{
    public string Id => "2026-01-11-rename-energy-field";

    public async Task ApplyAsync(IMongoDatabase db, CancellationToken ct = default)
    {
        var payments = db.GetCollection<BsonDocument>("payments");

        var filter = Builders<BsonDocument>.Filter.Exists("energyKwh", true);
        var update = Builders<BsonDocument>.Update
            .Rename("energyKwh", "energyConsumed");

        await payments.UpdateManyAsync(filter, update, cancellationToken: ct);
    }
}
