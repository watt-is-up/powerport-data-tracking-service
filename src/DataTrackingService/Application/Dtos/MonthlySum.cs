namespace DataTrackingService.Application.Dtos;

public sealed class MonthlySum
{
    public int Month { get; init; }
    public decimal Total { get; init; }
}
