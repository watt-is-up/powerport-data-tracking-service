using DataTrackingService.Domain.Models.Providers;

namespace DataTrackingService.Data.Mongo;

public interface ITenantRegistry
{
    TenantInfo Get(string providerId);
}

public class TenantRegistry : ITenantRegistry
{
    private readonly Dictionary<string, TenantInfo> _tenants = new();

    public TenantInfo Get(string providerId)
    {
        // Example: return shared or isolated info
        return _tenants.TryGetValue(providerId, out var tenant)
            ? tenant
            : new TenantInfo { Mode = TenantMode.Shared, DatabaseName = "shared_providers" };
    }
}

public class TenantInfo
{
    public TenantMode Mode { get; set; }
    public string DatabaseName { get; set; } = null!;
}
