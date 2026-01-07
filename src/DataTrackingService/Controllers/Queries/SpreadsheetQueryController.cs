using DataTrackingService.Application.Queries;
using DataTrackingService.Data.Mongo.Spreadsheets;
using Microsoft.AspNetCore.Mvc;

namespace DataTrackingService.Controllers.Queries;

[ApiController]
[Route("api/spreadsheets/{spreadsheetId}/query")]
public class SpreadsheetQueryController : ControllerBase
{
    private readonly UserSpreadsheetReadService _readService;

    public SpreadsheetQueryController(
        UserSpreadsheetReadService readService
    )
    {
        _readService = readService;
    }

    [HttpGet("sum-by-month")]
    public async Task<IActionResult> SumByMonth(
        string spreadsheetId,
        [FromQuery] string userId,
        [FromQuery] string column)
    {
        var result = await _readService
            .SumByMonthAsync(spreadsheetId, userId, column);

        return Ok(result);
    }

    // Raw data (table view)
    [HttpGet("rows")]
    public async Task<IActionResult> GetRows(
        string spreadsheetId,
        [FromQuery] string userId)
    {
        var rows = await _readService.GetRowsAsync(spreadsheetId, userId);
        return Ok(rows);
    }
}
