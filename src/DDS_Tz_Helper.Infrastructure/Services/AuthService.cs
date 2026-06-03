using System.Security.Cryptography;
using System.Text;
using DDS_Tz_Helper.Application.DTOs.Auth;
using DDS_Tz_Helper.Application.Interfaces;
using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DDS_Tz_Helper.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IDispatchDBContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(IDispatchDBContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = new LoginResponse();

        try
        {
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
            await RecordLogAsync("N/A", currentUser.Id, "LOGIN_HELPER", "Login_HELPER : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

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
            response.currentPlant = await GetCurrentPlantAsync(cancellationToken);
            response.company_name = "Dangote Cement Tanzania";
            response.loginStatus = "LoggedIn";
            return response;
        }
        catch (Exception ex)
        {
            response.msg = "Invalid or blocked username/password";
            response.status = false;
            response.excptn = ex.Message;
            return response;
        }
    }

    public async Task<LoginResponse> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
    {
        if (request?.user_id > 0)
        {
            await RecordLogAsync("N/A", request.user_id, "LOGOUT_HELPER", "LOGOUT_HELPER: ", cancellationToken);
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

    private async Task RecordLogAsync(string atc, int userId, string activity, string fieldName, CancellationToken cancellationToken)
    {
        var currentSession = await _context.atc_session.OrderByDescending(a => a.session_start).FirstOrDefaultAsync(cancellationToken);
       
        var transactionLog = new transaction_log
        {
            atc = atc,
            activity = activity,
            field_name = fieldName,
            field_value = DateTime.Now.ToString(),
            trx_date = DateTime.Now,
            user_id = userId
        };

        if (currentSession?.status == "open")
        {
            transactionLog.session_id = currentSession.Id;
        }

        _context.transaction_log.Add(transactionLog);
    }
}
