using MongoDB.Driver;

namespace DataTrackingService.Data.Mongo.Migrations;
public sealed class MongoMigrationRunner
{
    private readonly IEnumerable<IMongoMigration> _migrations;

    public MongoMigrationRunner(IEnumerable<IMongoMigration> migrations)
    {
        _migrations = migrations.OrderBy(m => m.Id);
    }

    public async Task RunAsync(IMongoDatabase db, CancellationToken ct = default)
    {
        var history = db.GetCollection<MongoMigrationRecord>("__migrations");

        var applied = await history
            .Find(FilterDefinition<MongoMigrationRecord>.Empty)
            .Project(x => x.Id)
            .ToListAsync(ct);

        foreach (var migration in _migrations)
        {
            if (applied.Contains(migration.Id))
                continue;

            await migration.ApplyAsync(db, ct);

            await history.InsertOneAsync(new MongoMigrationRecord
            {
                Id = migration.Id,
                AppliedAtUtc = DateTime.UtcNow
            }, cancellationToken: ct);
        }
    }
}
