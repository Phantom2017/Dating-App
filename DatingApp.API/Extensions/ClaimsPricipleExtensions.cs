using System.Security.Claims;

namespace DatingApp.API.Extensions
{
    public static class ClaimsPricipleExtensions
    {
        public static string GetUsername(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Name)?.Value;
        }

         public static int GetUserId(this ClaimsPrincipal principal)
        {
            return int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}