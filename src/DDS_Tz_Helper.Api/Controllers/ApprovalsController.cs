using DDS_Tz_Helper.Application.DTOs.Approvals;
using DDS_Tz_Helper.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

using DDS_Tz_Helper.Api.Common;
using Microsoft.AspNetCore.Authorization;

namespace DDS_Tz_Helper.Api.Controllers;

[ApiController]
[Authorize(Policy = "CheckerOnly")]
[Route("api/[controller]")]
public class ApprovalsController : ControllerBase
{
    private readonly IApprovalService _approvalService;

    public ApprovalsController(IApprovalService approvalService)
    {
        _approvalService = approvalService;
    }

    [HttpGet("Pending/{userId:int}")]
    public async Task<IActionResult> Pending(int userId, CancellationToken cancellationToken)
    {
        var response = await _approvalService.GetPendingApprovalsAsync(User.GetCurrentUserId(), cancellationToken);
        return Ok(response);
    }

    [HttpPost("Approve")]
    public async Task<IActionResult> Approve([FromBody] ApprovalActionRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var response = await _approvalService.ApproveAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("Decline")]
    public async Task<IActionResult> Decline([FromBody] ApprovalActionRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var response = await _approvalService.DeclineAsync(request, cancellationToken);
        return Ok(response);
    }
}
