namespace DataTrackingService.Controllers.Dtos;

public record InsertRowRequest(
    string UserId,
    Dictionary<string, object?> Values);
