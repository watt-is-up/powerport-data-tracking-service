using DataTrackingService.Domain.Models.Providers;
using DataTrackingService.Domain.Models.Spreadsheets;
using DataTrackingService.Domain.Models.Usage;
using MongoDB.Driver;

namespace DataTrackingService.Data.Mongo.Migrations;
public class MongoIndexInitializer
{

    private readonly IMongoDbContextFactory _contextFactory;

    public MongoIndexInitializer(IMongoDbContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }


    public async Task EnsureUserIndexesAsync(CancellationToken ct = default)
    {
        var delay = TimeSpan.FromSeconds(2);

        for (var attempt = 1; attempt <= 10; attempt++)
        {
            try
            {
                var userContext = _contextFactory.GetUserContext();

                await userContext.UserSpreadsheets.Indexes.CreateOneAsync(
                    new CreateIndexModel<UserSpreadsheet>(
                        Builders<UserSpreadsheet>.IndexKeys
                            .Ascending(x => x.UserId)
                            .Ascending(x => x.Name),
                        new CreateIndexOptions { Unique = true }
                    ),
                    cancellationToken: ct
                );

                await userContext.UserSpreadsheetRows.Indexes.CreateOneAsync(
                    new CreateIndexModel<UserSpreadsheetRow>(
                        Builders<UserSpreadsheetRow>.IndexKeys
                            .Ascending(x => x.SpreadsheetId)
                            .Ascending(x => x.UserId)
                    ),
                    cancellationToken: ct
                );


                return; // success
            }
            catch (MongoException ex) when (attempt < 10)
            {
                await Task.Delay(delay, ct);
            }
        }

        throw new InvalidOperationException("MongoDB not available after retries");
    }

    public async Task EnsureTenantIndexesAsync(Tenant tenant, CancellationToken ct = default)
    {
        var delay = TimeSpan.FromSeconds(2);

        for (var attempt = 1; attempt <= 10; attempt++)
        {
            try
            {
                // var tenantContext = _contextFactory.GetTenantContext(tenant.Id);

                // await tenantContext.ChargingSessions.Indexes.CreateOneAsync(
                //     new CreateIndexModel<ChargingSessionView>(
                //         Builders<ChargingSessionView>.IndexKeys.Ascending(x => x.SessionId),
                //         new CreateIndexOptions { Unique = true }
                //     )
                // );

                return; // success
            }
            catch (MongoException ex) when (attempt < 10)
            {
                await Task.Delay(delay, ct);
            }
        }

        throw new InvalidOperationException("MongoDB not available after retries");
    }

}
