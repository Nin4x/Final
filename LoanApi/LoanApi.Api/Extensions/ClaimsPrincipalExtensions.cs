using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LoanApi.Domain.Enums;

namespace LoanApi.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var idClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub) ?? principal.FindFirst(ClaimTypes.NameIdentifier);
        if (idClaim is null)
        {
            throw new InvalidOperationException("User identifier claim is missing.");
        }

        return Guid.Parse(idClaim.Value);
    }

    public static UserRole GetUserRole(this ClaimsPrincipal principal)
    {
        var roleClaim = principal.FindFirst(ClaimTypes.Role);
        if (roleClaim is null || !Enum.TryParse<UserRole>(roleClaim.Value, out var role))
        {
            throw new InvalidOperationException("User role claim is missing or invalid.");
        }

        return role;
    }
}
