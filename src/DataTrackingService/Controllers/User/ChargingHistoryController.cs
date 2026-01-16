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

    /// <summary>
    /// Get the complete charging history of a selected user
    /// </summary>
    /// <param name="userId">The ID of the user you want charging history from</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetUserChargingHistory(string userId)
    {
        var sessions = await _readService.GetUserSessionsAsync(userId);
        return Ok(sessions);
    }
}
