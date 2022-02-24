using System.Security.Claims;

namespace API.Extensionos;

public static class ClaimsPrincipalExtensions
{
    public static string RetrieveEmailFromPrincipal(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Email);
    }
}
