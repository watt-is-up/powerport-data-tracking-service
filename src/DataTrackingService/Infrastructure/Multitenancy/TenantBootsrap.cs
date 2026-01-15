using DataTrackingService.Domain.Models.Providers;
using Microsoft.Extensions.Configuration;

namespace DataTrackingService.Infrastructure.Multitenancy;
public static class TenantBootstrap
{
    public static List<Tenant> GetMockTenants() => new()
    {
        new Tenant
        {
            Id = "11111111-1111-1111-1111-111111111111",
            Name = "shared",
            Mode = TenantMode.Isolated,
            DatabaseConnection = new DatabaseConnection
            {
                User = "data_tracking_shared_user",
                Password = "secretpassword",
                DatabaseName = "data_tracking_shared_db"
            }
        },
        new Tenant
        {
            Id = "5a8d3c1f-9e42-4b7d-8f06-2c91e7a4d6b5",
            Name = "ina",
            Mode = TenantMode.Isolated,
            DatabaseConnection = new DatabaseConnection
            {
                User = "data_tracking_ina_user",
                Password = "secretpassword",
                DatabaseName = "data_tracking_ina_db"
            }
        },
        new Tenant
        {
            Id = "91c4b7a2-6d3e-4f89-a5c1-0e2d8b6f7a43",
            Name = "petrol",
            Mode = TenantMode.Isolated,
            DatabaseConnection = new DatabaseConnection
            {
                User = "data_tracking_petrol_user",
                Password = "secretpassword",
                DatabaseName = "data_tracking_petrol_db"
            }
        },
        new Tenant
        {
            Id = "2e9f6a7b-4c1d-4e3a-9b8f-6d1a7c5e4b04",
            Name = "makpetrol",
            Mode = TenantMode.Isolated,
            DatabaseConnection = new DatabaseConnection
            {
                User = "data_tracking_makpetrol_user",
                Password = "secretpassword",
                DatabaseName = "data_tracking_makpetrol_db"
            }
        }
    };

    public static List<Tenant> LoadFromConfiguration(IConfiguration configuration)
    {
        var tenantsSection = configuration.GetSection(TenantsOptions.SectionName);
        var tenants = new List<Tenant>();

        foreach (var child in tenantsSection.GetChildren())
        {
            var name = child.Key;
            var id = child["Id"];
            var mode = child["Mode"];
            var user = child["DatabaseConnection:User"];
            var password = child["DatabaseConnection:Password"];
            var databaseName = child["DatabaseConnection:DatabaseName"];

            if (string.IsNullOrWhiteSpace(id))
                throw new InvalidOperationException($"Tenant '{name}' is missing Id");

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new InvalidOperationException($"Tenant '{name}' is missing DatabaseConnection:DatabaseName");

            tenants.Add(new Tenant
            {
                Id = id,
                Name = name,
                Mode = Enum.TryParse<TenantMode>(mode, ignoreCase: true, out var parsed) 
                    ? parsed 
                    : TenantMode.Isolated,
                DatabaseConnection = new DatabaseConnection
                {
                    User = user ?? $"data_tracking_{name}_user",
                    Password = password ?? "secretpassword",
                    DatabaseName = databaseName
                }
            });
        }

        if (tenants.Count == 0)
            throw new InvalidOperationException("No tenants configured in 'Tenants' section");

        return tenants;
    }
}

public sealed class TenantsOptions
{
    public const string SectionName = "Tenants";
    public const string SharedTenantName = "shared";

}

public sealed class TenantConfig
{
    public string Id { get; set; } = default!;
    public string Mode { get; set; } = "Isolated";
    public DatabaseConnectionConfig DatabaseConnection { get; set; } = default!;
}

public sealed class DatabaseConnectionConfig
{
    public string User { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string DatabaseName { get; set; } = default!;
}
