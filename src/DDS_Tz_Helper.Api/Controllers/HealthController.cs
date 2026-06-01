using Microsoft.AspNetCore.Mvc;

namespace DDS_Tz_Helper.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "Healthy" });
}
