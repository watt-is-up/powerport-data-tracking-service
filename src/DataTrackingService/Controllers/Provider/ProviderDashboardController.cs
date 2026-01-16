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


    /// <summary>
    /// Get the data about monthy revenue
    /// </summary>
    /// <param name="providerId">The ID of the provider you are interested in</param>
    /// <returns></returns>
    [HttpGet("revenue/monthly")]
    public async Task<IActionResult> GetMonthlyRevenue(string providerId)
    {
        var data = await _chargingSessionQueryService.GetMonthlyRevenue(providerId);
        return Ok(data);
    }

    /// <summary>
    /// Get the total power usage of a provider
    /// </summary>
    /// <param name="providerId">The ID of the provider you are interested in</param>
    /// <param name="from">The starting date and time</param>
    /// <param name="to">The ending date and time</param>
    /// <returns></returns>
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
