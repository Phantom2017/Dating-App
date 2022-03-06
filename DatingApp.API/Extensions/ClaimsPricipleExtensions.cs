using System.Security.Claims;

namespace DatingApp.API.Extensions
{
    public static class ClaimsPricipleExtensions
    {
        public static string GetUsername(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}