using DDS_Tz_Helper.Application.DTOs.ATCGrade;
using DDS_Tz_Helper.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DDS_Tz_Helper.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ATCGradeController : ControllerBase
{
    private readonly IATCGradeService _atcGradeService;

    public ATCGradeController(IATCGradeService atcGradeService)
    {
        _atcGradeService = atcGradeService;
    }

    [HttpPost("DoChangeATCGrade")]
    public async Task<IActionResult> DoChangeATCGrade([FromBody] DoChangeATCGradeRequest request, CancellationToken cancellationToken)
    {
        var response = await _atcGradeService.DoChangeATCGradeAsync(request, cancellationToken);
        return Ok(response);
    }
}
