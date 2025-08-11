using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DearFab_Model.Utils;

public class UserUtil
{
    public static Guid? GetAccountId(HttpContext httpContext)
    {
        if (httpContext == null || httpContext.User == null)
        {
            return null;
        }

        var nameIdentifierClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (nameIdentifierClaim == null)
        {
            return null;
        }

        if (!Guid.TryParse(nameIdentifierClaim.Value, out Guid accountId))
        {
            throw new BadHttpRequestException(nameIdentifierClaim.Value);

        }
        return accountId;
    }
}