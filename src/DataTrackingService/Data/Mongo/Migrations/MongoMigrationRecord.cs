namespace DataTrackingService.Data.Mongo.Migrations;
public sealed class MongoMigrationRecord
{
    public string Id { get; init; } = default!;
    public DateTime AppliedAtUtc { get; init; }
}
