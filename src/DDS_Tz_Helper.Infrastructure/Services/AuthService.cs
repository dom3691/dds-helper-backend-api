using System.Security.Cryptography;
using System.Text;
using DDS_Tz_Helper.Application.DTOs.Auth;
using DDS_Tz_Helper.Application.Interfaces;
using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace DDS_Tz_Helper.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IDispatchDBContext _context;
    private readonly IConfiguration _configuration;
    private readonly ITransactionLogService _transactionLogService;

    public AuthService(IDispatchDBContext context, IConfiguration configuration, ITransactionLogService transactionLogService)
    {
        _context = context;
        _configuration = configuration;
        _transactionLogService = transactionLogService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = new LoginResponse();

        if (request is null || string.IsNullOrWhiteSpace(request.username) || string.IsNullOrWhiteSpace(request.password))
        {
            response.msg = "Invalid or blocked username/password";
            response.excptn = "Invalid or blocked username/password";
            response.status = false;
            return response;
        }

        var hashedPassword = Hash(request.password);
        var currentUser = await _context.user.FirstOrDefaultAsync(u => u.username == request.username && u.password == hashedPassword && u.active == true && u.approved == true && u.username != "system", cancellationToken);

        if (currentUser is null || currentUser.status != "ACTIVE")
        {
            response.msg = "Invalid or blocked username/password";
            response.excptn = "Invalid or blocked username/password";
            response.status = false;
            return response;
        }

        currentUser.last_login = DateTime.Now;
        await _transactionLogService.RecordLogAsync("N/A", currentUser.Id, "LOGIN_HELPER", "Login_HELPER : ", cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var currentPlant = await GetCurrentPlantAsync(cancellationToken);
        var tokenExpiresAt = DateTime.UtcNow.AddMinutes(GetJwtExpiryMinutes());

        response.msg = "";
        response.status = true;
        response.username = currentUser.username;
        response.user_id = currentUser.Id;
        response.role_id = currentUser.role_id ?? 0;
        response.user_status = currentUser.status;
        response.active = currentUser.active == true ? 1 : 0;
        response.admin = currentUser.admin == true;
        response.can_approve = currentUser.can_approve == true;
        response.canApproveTare = CanApproveTare(currentUser.username) ? "cat" : "cnat";
        response.currentPlant = currentPlant;
        response.company_name = "Dangote Cement Tanzania";
        response.loginStatus = "LoggedIn";
        response.token = CreateJwtToken(currentUser, currentPlant, response.can_approve, tokenExpiresAt);
        response.tokenExpiresAt = tokenExpiresAt;
        return response;
    }

    public async Task<LoginResponse> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
    {
        if (request?.user_id > 0)
        {
            await _transactionLogService.RecordLogAsync("N/A", request.user_id, "LOGOUT_HELPER", "LOGOUT_HELPER: ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return new LoginResponse { status = true, loginStatus = "LoggedOut" };
    }

    private static string Hash(string password)
    {
        var bytes = new UTF8Encoding().GetBytes(password);
        var hashBytes = MD5.HashData(bytes);
        return Convert.ToBase64String(hashBytes);
    }

    private bool CanApproveTare(string username)
    {
        var approvers = _configuration["Authentication:CheckerUsers"] ?? string.Empty;
        return approvers
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(approver => string.Equals(username, approver, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<string> GetCurrentPlantAsync(CancellationToken cancellationToken)
    {
        var plant = await _context.system_setting.FirstOrDefaultAsync(x => x.switch_over == "auto", cancellationToken);
        return plant?.plant_code == "2000" ? "GBOKO" : "OTHERS";
    }

    private string CreateJwtToken(user currentUser, string currentPlant, bool canApprove, DateTime expiresAt)
    {
        var key = _configuration["Jwt:Key"] ?? string.Empty;
        if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
        {
            throw new InvalidOperationException("Jwt:Key must be configured and must be at least 32 characters long.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, currentUser.Id.ToString()),
            new Claim(ClaimTypes.Name, currentUser.username ?? string.Empty),
            new Claim("RoleId", (currentUser.role_id ?? 0).ToString()),
            new Claim("Plant", currentPlant),
            new Claim("CanApprove", canApprove ? "true" : "false")
        };

        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "DDS_Tz_Helper",
            audience: _configuration["Jwt:Audience"] ?? "DDS_Tz_Helper.Web",
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int GetJwtExpiryMinutes()
    {
        var minutes = int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var configuredMinutes)
            ? configuredMinutes
            : 480;

        return minutes > 0 ? minutes : 480;
    }
}
