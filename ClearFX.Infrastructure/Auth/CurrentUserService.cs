using System.Security.Claims;
using ClearFX.Application.Common;
using Microsoft.AspNetCore.Http;

namespace ClearFX.Infrastructure.Auth;

public class CurrentUserService(IHttpContextAccessor httpContext) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            var user = httpContext.HttpContext?.User
                ?? throw new UnauthorizedAccessException("No active HTTP context was found.");

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException("Authenticated user id was not found.");

            return Guid.TryParse(userId, out var parsedUserId)
                ? parsedUserId
                : throw new UnauthorizedAccessException("Authenticated user id is invalid.");
        }
    }
}
