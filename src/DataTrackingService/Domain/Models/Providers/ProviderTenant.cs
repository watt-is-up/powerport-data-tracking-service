namespace DataTrackingService.Domain.Models.Providers;

public class ProviderTenant
{
    public string TenantId { get; set; } = null!;
    public string ProviderId { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public TenantMode Mode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
