using DDS_Tz_Helper.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DDS_Tz_Helper.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActivityController : ControllerBase
{
    private readonly IActivityService _activityService;

    public ActivityController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    [HttpGet("Recent")]
    public async Task<IActionResult> Recent([FromQuery] int count = 10, CancellationToken cancellationToken = default)
    {
        var response = await _activityService.GetRecentActivitiesAsync(count, cancellationToken);
        return Ok(response);
    }
}
