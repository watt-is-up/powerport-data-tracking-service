using DataTrackingService.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace DataTrackingService.Controllers.User;

[ApiController]
[Route("api/users/{userId}/charging-sessions")]
public class ChargingHistoryController : ControllerBase
{
    private readonly ChargingSessionReadService _readService;

    public ChargingHistoryController(ChargingSessionReadService readService)
    {
        _readService = readService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserChargingHistory(string userId)
    {
        var sessions = await _readService.GetUserSessionsAsync(userId);
        return Ok(sessions);
    }
}
