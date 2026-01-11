namespace DataTrackingService.Domain.Models.Providers;

public class Tenant
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required TenantMode Mode { get; set; }
    public required DatabaseConnection DatabaseConnection { get; set; }
}

public class DatabaseConnection
{
    public required string User { get; set;}
    public required string Password { get; set;}
    public required string DatabaseName { get; set;}

}