using DDS_Tz_Helper.Application.DTOs.Receiver;
using DDS_Tz_Helper.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

using DDS_Tz_Helper.Api.Common;
using Microsoft.AspNetCore.Authorization;

namespace DDS_Tz_Helper.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ReceiverController : ControllerBase
{
    private readonly IReceiverService _receiverService;

    public ReceiverController(IReceiverService receiverService)
    {
        _receiverService = receiverService;
    }

    [HttpPost("DoUpdateReceiver")]
    public async Task<IActionResult> DoUpdateReceiver([FromBody] DoUpdateReceiverRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var response = await _receiverService.DoUpdateReceiverAsync(request, cancellationToken);
        return Ok(response);
    }
}
