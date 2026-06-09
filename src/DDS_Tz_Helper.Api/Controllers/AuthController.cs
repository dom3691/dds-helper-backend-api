using DDS_Tz_Helper.Application.DTOs.Auth;
using DDS_Tz_Helper.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

using DDS_Tz_Helper.Api.Common;
using Microsoft.AspNetCore.Authorization;

namespace DDS_Tz_Helper.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("LoginOff")]
    [Authorize]
    public async Task<IActionResult> LoginOff([FromBody] LogoutRequest request, CancellationToken cancellationToken)
    {
        request ??= new LogoutRequest();
        request.user_id = User.GetCurrentUserId();
        var response = await _authService.LogoutAsync(request, cancellationToken);
        return Ok(response);
    }
}
