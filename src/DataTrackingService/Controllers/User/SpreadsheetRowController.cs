using DataTrackingService.Application.Commands;
using DataTrackingService.Domain.Models.Spreadsheets;
using Microsoft.AspNetCore.Mvc;

namespace DataTrackingService.Controllers.User;

[ApiController]
[Route("api/spreadsheets/{spreadsheetId}/rows")]
public class SpreadsheetRowsController : ControllerBase
{
    private readonly UserSpreadsheetWriteService _writeService;

    public SpreadsheetRowsController(UserSpreadsheetWriteService service)
    {
        _writeService = service;
    }

    [HttpPost]
    public async Task<IActionResult> InsertRow(
        string spreadsheetId,
        [FromBody] Dtos.InsertRowRequest req)
    {
        var row = new UserSpreadsheetRow
        {
            SpreadsheetId = spreadsheetId,
            UserId = req.UserId,
            Values = req.Values
        };

        await _writeService.AddRowAsync(
            spreadsheetId,
            req.UserId,
            req.Values);
            
        return NoContent();
    }
}
