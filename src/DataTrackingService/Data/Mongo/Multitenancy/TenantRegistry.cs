using DataTrackingService.Domain.Models.Providers;

namespace DataTrackingService.Data.Mongo.Multitenancy;
public interface ITenantRegistry
{
    Tenant? Get(string providerId);
    List<Tenant> GetTenants();
}

public class TenantRegistry : ITenantRegistry
{
    private readonly Dictionary<string, Tenant> _tenants = new();

    public TenantRegistry(IEnumerable<Tenant> tenants)
    {
        _tenants = tenants.ToDictionary(t => t.Id);
    }

    public Tenant? Get(string providerId)
    {
        // Example: return shared or isolated info
        _tenants.TryGetValue(providerId, out var tenant);
        return tenant;
    }

    public List<Tenant> GetTenants()
    {
        return _tenants.Values.ToList();
    }
}
