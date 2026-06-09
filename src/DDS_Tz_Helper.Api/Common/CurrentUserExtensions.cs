using System.Security.Claims;

namespace DDS_Tz_Helper.Api.Common;

public static class CurrentUserExtensions
{
    public static int GetCurrentUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(value, out var userId) && userId > 0)
        {
            return userId;
        }

        throw new UnauthorizedAccessException("A valid user session is required.");
    }
}
