using DataTrackingService.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace DataTrackingService.Controllers.User;

[ApiController]
[Route("api/users/{userId}/usage")]
public class UserUsageController : ControllerBase
{
    private readonly UserChargingSessionQueryService _queryService;

    public UserUsageController(UserChargingSessionQueryService queryService)
    {
        _queryService = queryService;
    }

    /// <summary>
    /// Get the monthy usage of a user
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <returns></returns>
    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlyUsage(string userId)
    {
        var usage = await _queryService.GetMonthlyUsage(userId);
        return Ok(usage);
    }
}
