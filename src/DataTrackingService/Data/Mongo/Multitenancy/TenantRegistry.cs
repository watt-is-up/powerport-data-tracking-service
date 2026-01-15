using DataTrackingService.Domain.Models.Providers;
using DataTrackingService.Infrastructure.Multitenancy;


namespace DataTrackingService.Data.Mongo.Multitenancy;
public interface ITenantRegistry
{
    Tenant? Get(string providerId);
    List<Tenant> GetTenants();
    Tenant GetTenantByName(string name);
    Tenant GetSharedTenant();
}

public class TenantRegistry : ITenantRegistry
{
    private readonly Dictionary<string, Tenant> _tenants = new();
    private readonly string _SharedTenantId;

    public TenantRegistry(IEnumerable<Tenant> tenants)
    {
        _tenants = tenants.ToDictionary(t => t.Id);
        _SharedTenantId = GetTenantByName(TenantsOptions.SharedTenantName)?.Id 
            ?? throw new InvalidOperationException($"Shared tenant '{TenantsOptions.SharedTenantName}' not found");

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

    public Tenant GetTenantByName(string name)
    {
        var tenant = _tenants.Values.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (tenant == null)
        {
            throw new KeyNotFoundException($"Tenant with name '{name}' not found.");
        }
        return tenant;
    }

    public Tenant GetSharedTenant()
    {
        return Get(_SharedTenantId) 
            ?? throw new InvalidOperationException("Shared tenant not found in registry.");
    }


}

