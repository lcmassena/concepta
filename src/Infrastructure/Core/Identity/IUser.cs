using System.Collections.Generic;
using System.Security.Claims;

namespace Massena.Infrastructure.Core.Identity
{
    public interface IUser
    {
        string Name { get; }
        bool IsAuthenticated();
        IEnumerable<Claim> GetClaimsIdentity();
    }
}
