using DDS_Tz_Helper.Api.Common;
using DDS_Tz_Helper.Application.DTOs.Reports;
using DDS_Tz_Helper.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DDS_Tz_Helper.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportsService _reportsService;

    public ReportsController(IReportsService reportsService)
    {
        _reportsService = reportsService;
    }

    [HttpGet("GetReportDataFilter")]
    public async Task<IActionResult> GetReportDataFilter([FromQuery] GateReportFilterRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var report = await _reportsService.GetReportDataFilterAsync(request, cancellationToken);
        return Ok(new ReportDataResponse<object> { data = report.Cast<object>().ToList() });
    }

    [HttpGet("GetMaterialReport")]
    public async Task<IActionResult> GetMaterialReport([FromQuery] MaterialReportRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var report = await _reportsService.GetMaterialReportAsync(request, cancellationToken);
        return Ok(new ReportDataResponse<object> { data = report.Cast<object>().ToList() });
    }

    [HttpGet("GetTripDetailsReportDataFilter")]
    public async Task<IActionResult> GetTripDetailsReportDataFilter([FromQuery] TripDetailsReportRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();

        if (request.trxType == "Trip Creation")
        {
            var creationReport = await _reportsService.GetTripCreationDetailsReportAsync(request, cancellationToken);
            return Ok(new ReportDataResponse<object> { data = creationReport.Cast<object>().ToList() });
        }

        if (request.trxType == "Trip Closure")
        {
            var closureReport = await _reportsService.GetTripClosureDetailsReportAsync(request, cancellationToken);
            return Ok(new ReportDataResponse<object> { data = closureReport.Cast<object>().ToList() });
        }

        return BadRequest(new { msg = "Unsupported trip report type.", status = false });
    }

    [HttpGet("GetReportPostingErrors")]
    public async Task<IActionResult> GetReportPostingErrors([FromQuery] PostingErrorsReportRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var report = await _reportsService.GetReportPostingErrorsAsync(request, cancellationToken);
        return Ok(new ReportDataResponse<object> { data = report.Cast<object>().ToList() });
    }

    [HttpGet("GetReportSTOPostingErrors")]
    public async Task<IActionResult> GetReportSTOPostingErrors([FromQuery] PostingErrorsReportRequest request, CancellationToken cancellationToken)
    {
        request.user_id = User.GetCurrentUserId();
        var report = await _reportsService.GetReportSTOPostingErrorsAsync(request, cancellationToken);
        return Ok(new ReportDataResponse<object> { data = report.Cast<object>().ToList() });
    }
}
