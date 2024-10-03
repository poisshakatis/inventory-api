using System.Security.Claims;

namespace WebApp.Utils;

public static class IdentityUtils
{
    public static int GetId(this ClaimsPrincipal user)
    {
        return Convert.ToInt32(user.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}