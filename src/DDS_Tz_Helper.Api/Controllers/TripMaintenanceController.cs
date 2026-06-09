using DDS_Tz_Helper.Application.DTOs.TripMaintenance;
using DDS_Tz_Helper.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

using DDS_Tz_Helper.Api.Common;
using Microsoft.AspNetCore.Authorization;

namespace DDS_Tz_Helper.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TripMaintenanceController : ControllerBase
{
    private readonly ITripMaintenanceService _tripMaintenanceService;

    public TripMaintenanceController(ITripMaintenanceService tripMaintenanceService)
    {
        _tripMaintenanceService = tripMaintenanceService;
    }

    [HttpPost("DoUpdateDriver")]
    public async Task<IActionResult> DoUpdateDriver([FromBody] DoUpdateDriverRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var response = await _tripMaintenanceService.DoUpdateDriverAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("DoUpdateDestination")]
    public async Task<IActionResult> DoUpdateDestination([FromBody] DoUpdateDestinationRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var response = await _tripMaintenanceService.DoUpdateDestinationAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("DoUpdateTrip")]
    public async Task<IActionResult> DoUpdateTrip([FromBody] DoUpdateTripRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var response = await _tripMaintenanceService.DoUpdateTripAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("AddOnlineTrip")]
    public async Task<IActionResult> AddOnlineTrip([FromBody] AddOnlineTripRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var response = await _tripMaintenanceService.AddOnlineTripAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("AddDriverManually")]
    public async Task<IActionResult> AddDriverManually([FromBody] AddDriverManuallyRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var response = await _tripMaintenanceService.AddDriverManuallyAsync(request, cancellationToken);
        return Ok(response);
    }
}
