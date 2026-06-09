using DDS_Tz_Helper.Application.DTOs.ATCDetails;
using DDS_Tz_Helper.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

using DDS_Tz_Helper.Api.Common;
using Microsoft.AspNetCore.Authorization;

namespace DDS_Tz_Helper.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ATCDetailsController : ControllerBase
{
    private readonly IATCDetailsService _atcDetailsService;

    public ATCDetailsController(IATCDetailsService atcDetailsService)
    {
        _atcDetailsService = atcDetailsService;
    }

    [HttpPost("DoFetchATCDetails")]
    public async Task<IActionResult> DoFetchATCDetails([FromBody] FetchATCDetailsRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var response = await _atcDetailsService.DoFetchATCDetailsAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("DoFetchATCDetailsGate")]
    public async Task<IActionResult> DoFetchATCDetailsGate([FromBody] FetchATCDetailsRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var response = await _atcDetailsService.DoFetchATCDetailsGateAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("DoFetchATcForArchiveDetails")]
    public async Task<IActionResult> DoFetchATcForArchiveDetails([FromBody] FetchATCDetailsRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var response = await _atcDetailsService.DoFetchATcForArchiveDetailsAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("DoFetchATcForTareChangeDetails")]
    public async Task<IActionResult> DoFetchATcForTareChangeDetails([FromBody] FetchATCDetailsRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var response = await _atcDetailsService.DoFetchATcForTareChangeDetailsAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("DoFetchATCDetailsForQuantityUpdate")]
    public async Task<IActionResult> DoFetchATCDetailsForQuantityUpdate([FromBody] FetchATCDetailsRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var response = await _atcDetailsService.DoFetchATCDetailsForQuantityUpdateAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("DoFetchATcForGrossModif")]
    public async Task<IActionResult> DoFetchATcForGrossModif([FromBody] FetchATCDetailsRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var response = await _atcDetailsService.DoFetchATcForGrossModifAsync(request, cancellationToken);
        return Ok(response);
    }
}
