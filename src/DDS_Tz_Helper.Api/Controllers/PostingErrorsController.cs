using DDS_Tz_Helper.Application.DTOs.PostingErrors;
using DDS_Tz_Helper.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DDS_Tz_Helper.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostingErrorsController : ControllerBase
{
    private readonly IPostingErrorsService _postingErrorsService;

    public PostingErrorsController(IPostingErrorsService postingErrorsService)
    {
        _postingErrorsService = postingErrorsService;
    }

    [HttpPost("DoUpdateOfflineTripTForRepush")]
    public async Task<IActionResult> DoUpdateOfflineTripTForRepush([FromBody] AtcPostingErrorRequest request, CancellationToken cancellationToken)
    {
        var response = await _postingErrorsService.DoUpdateOfflineTripTForRepushAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("DoUpdateMolasses")]
    public async Task<IActionResult> DoUpdateMolasses([FromBody] AtcPostingErrorRequest request, CancellationToken cancellationToken)
    {
        var response = await _postingErrorsService.DoUpdateMolassesAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("DoRemoveTripIDForCTES")]
    public async Task<IActionResult> DoRemoveTripIDForCTES([FromBody] AtcPostingErrorRequest request, CancellationToken cancellationToken)
    {
        var response = await _postingErrorsService.DoRemoveTripIDForCTESAsync(request, cancellationToken);
        return Ok(response);
    }
}
