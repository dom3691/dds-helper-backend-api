using DDS_Tz_Helper.Application.DTOs.ATCSwap;
using DDS_Tz_Helper.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DDS_Tz_Helper.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ATCSwapController : ControllerBase
{
    private readonly IATCSwapService _atcSwapService;

    public ATCSwapController(IATCSwapService atcSwapService)
    {
        _atcSwapService = atcSwapService;
    }

    [HttpPost("DoATCSwap")]
    public async Task<IActionResult> DoATCSwap([FromBody] DoATCSwapRequest request, CancellationToken cancellationToken)
    {
        var response = await _atcSwapService.DoATCSwapAsync(request, cancellationToken);
        return Ok(response);
    }
}
