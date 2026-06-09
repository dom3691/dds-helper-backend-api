using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;

namespace DDS_Tz_Helper.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "Healthy" });
}
