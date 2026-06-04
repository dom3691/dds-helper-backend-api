using DDS_Tz_Helper.Application.DTOs.Approvals;
using DDS_Tz_Helper.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DDS_Tz_Helper.Api.Controllers;

[ApiController]
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
        var response = await _approvalService.GetPendingApprovalsAsync(userId, cancellationToken);
        return Ok(response);
    }

    [HttpPost("Approve")]
    public async Task<IActionResult> Approve([FromBody] ApprovalActionRequest request, CancellationToken cancellationToken)
    {
        var response = await _approvalService.ApproveAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("Decline")]
    public async Task<IActionResult> Decline([FromBody] ApprovalActionRequest request, CancellationToken cancellationToken)
    {
        var response = await _approvalService.DeclineAsync(request, cancellationToken);
        return Ok(response);
    }
}
