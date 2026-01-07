using DataTrackingService.Domain.Models.Spreadsheets;

namespace DataTrackingService.Controllers.Dtos;

public record AddColumnRequest(
    string UserId,
    string Name,
    SpreadsheetColumnType Type);
