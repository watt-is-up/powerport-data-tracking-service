using DataTrackingService.Domain.Models.Providers;

namespace DataTrackingService.Infrastructure.Multitenancy;
public static class TenantBootstrap
{
    public static List<Tenant> GetMockTenants() => new()
    {
        new Tenant
        {
            Id = "b7e6c9d1-5e7a-4c9d-8a31-1c7f2d9b8a01",
            Name = "Provider A",
            Mode = TenantMode.Isolated,
            DatabaseConnection = new DatabaseConnection
            {
                User = "providera",
                Password = "secretpassword",
                DatabaseName = "billing_provider_a"
            }
        },
        new Tenant
        {
            Id = "e8b1c7d6-5f4a-4e9b-8c2d-1a7f6b9e5c02",
            Name = "Provider B",
            Mode = TenantMode.Isolated,
            DatabaseConnection = new DatabaseConnection
            {
                User = "providerb",
                Password = "secretpassword",
                DatabaseName = "billing_provider_b"
            }
        }
    };
}
