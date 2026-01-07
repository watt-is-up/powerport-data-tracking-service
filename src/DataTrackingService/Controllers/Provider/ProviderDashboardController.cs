using DataTrackingService.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace DataTrackingService.Controllers.Provider;

[ApiController]
[Route("api/providers/{providerId}/dashboard")]
public class ProviderDashboardController : ControllerBase
{
    private readonly ProviderChargingSessionQueryService _chargingSessionQueryService;

    public ProviderDashboardController(
        ProviderChargingSessionQueryService chargingSessionQueryService)
    {
        _chargingSessionQueryService = chargingSessionQueryService;
    }

    [HttpGet("revenue/monthly")]
    public async Task<IActionResult> GetMonthlyRevenue(string providerId)
    {
        var data = await _chargingSessionQueryService.GetMonthlyRevenue(providerId);
        return Ok(data);
    }

    [HttpGet("power")]
    public async Task<IActionResult> GetPowerUsage(
        string providerId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var data = await _chargingSessionQueryService.GetPowerUsage(providerId, from, to);
        return Ok(data);
    }
}
