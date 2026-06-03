using DDS_Tz_Helper.Application.DTOs.Auth;

namespace DDS_Tz_Helper.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default);
}
