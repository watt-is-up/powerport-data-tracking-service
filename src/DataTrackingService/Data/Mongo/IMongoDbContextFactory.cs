namespace DataTrackingService.Data.Mongo;

public interface IMongoDbContextFactory
{
    UserMongoDbContext GetUserContext();
    TenantMongoDbContext GetTenantContext(string providerId);
}
