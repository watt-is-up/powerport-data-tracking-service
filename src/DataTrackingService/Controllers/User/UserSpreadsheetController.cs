using DataTrackingService.Application.Commands;
using DataTrackingService.Domain.Models.Spreadsheets;
using Microsoft.AspNetCore.Mvc;

namespace DataTrackingService.Controllers.User;

[ApiController]
[Route("api/spreadsheets")]
public class UserSpreadsheetsController : ControllerBase
{
    private readonly UserSpreadsheetWriteService _writeService;

    public UserSpreadsheetsController(UserSpreadsheetWriteService service)
    {
        _writeService = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Dtos.CreateSpreadsheetRequest req)
    {
        var sheet = await _writeService.CreateSpreadsheetAsync(req.UserId, req.Name);
        return Ok(new { SpreadsheetId = sheet.Id });
    }

    [HttpPost("{id}/columns")]
    public async Task<IActionResult> AddColumn(
        string id,
        [FromBody] Dtos.AddColumnRequest req)
    {
        var column = await _writeService.AddColumnAsync(
            id,
            req.UserId,
            new SpreadsheetColumn { Name = req.Name, Type = req.Type });

        return Ok(new { column.ColumnId });
    }
}
