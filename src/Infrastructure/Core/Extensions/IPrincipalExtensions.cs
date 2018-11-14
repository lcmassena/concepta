using IdentityModel;
using System;
using System.Security.Claims;

namespace Massena.Infrastructure.Core.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static String GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            String userId = null;

            if (claimsPrincipal != null)
            {
                Claim claim = claimsPrincipal.FindFirst(JwtClaimTypes.Subject);

                if (claim == null)
                {
                    claim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);

                    if (claim == null)
                        claim = claimsPrincipal.FindFirst("client_id");
                }

                userId = claim?.Value;
            }

            return userId;
        }
    }
}
