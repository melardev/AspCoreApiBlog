using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace BlogDotNet.Infrastructure.Extensions
{
    public static class ClaimsExtension
    {
        static string GetUserEmail(this ClaimsIdentity identity)
        {
            return identity.Claims?.FirstOrDefault(c => c.Type == "EndToEnd.Models.RegisterViewModel.Email")?.Value;
        }

        public static string GetUserEmail(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            return claimsIdentity != null ? GetUserEmail(claimsIdentity) : "";
        }

        static string GetUserNameIdentifier(this ClaimsIdentity identity)
        {
            return identity.Claims?.FirstOrDefault(c => c.Type == "EndToEnd.Models.RegisterViewModel.FirstName")?.Value;
        }

        public static string GetUserNameIdentifier(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            return claimsIdentity != null ? GetUserNameIdentifier(claimsIdentity) : "";
        }
        public static string GetFirstName(this IIdentity identity)
        {
            /*
            FinCoreDB db = new FinCoreDB();

            var userId = identity.GetUserId();
            var person = db.Users.FirstOrDefault(u => u.Id.Equals(userId)).FirstName;
            return person;
            */
            return "";
        }
    }
}