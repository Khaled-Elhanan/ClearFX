using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ClearFX.Application.Common;
using ClearFX.Domain.Entities;
using ClearFX.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ClearFX.Application.Features.Auth.Commands;

public class RefreshTokenCommandHandler(
    IRepository<User> userRepo,
    IUnitOfWork uow,
    IJwtService jwtService,
    IConfiguration config)
    : IRequestHandler<RefreshTokenCommand, LoginResult>
{
    public async Task<LoginResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 1. Extract userId from the expired access token
        var userId = ExtractUserIdFromToken(request.AccessToken, config);
        if (userId is null)
            throw new UnauthorizedAccessException("Invalid access token.");

        // 2. Load user
        var users = await userRepo.FindAsync(u => u.Id == userId, cancellationToken);
        var user  = users.FirstOrDefault()
            ?? throw new UnauthorizedAccessException("User not found.");

        // 3. Check account still active
        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is deactivated.");

        // 4. Validate refresh token exists and not expired
        if (user.RefreshToken is null || user.RefreshTokenExpiry is null)
            throw new UnauthorizedAccessException("No active refresh token.");

        if (user.RefreshTokenExpiry < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token has expired. Please login again.");

        // 5. Compare hashed refresh token
        var incoming = HashRefreshToken(request.RefreshToken);
        if (!string.Equals(user.RefreshToken, incoming, StringComparison.Ordinal))
            throw new UnauthorizedAccessException("Invalid refresh token.");

        // 6. Rotate — generate new token pair
        var (newAccessToken, newRefreshToken) = jwtService.GenerateTokens(user);

        user.RefreshToken       = HashRefreshToken(newRefreshToken);
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await uow.SaveChangesAsync(cancellationToken);

        return new LoginResult(newAccessToken, newRefreshToken, user.Role.ToString(), 3600);
    }

    public static string HashRefreshToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static Guid? ExtractUserIdFromToken(string accessToken, IConfiguration config)
    {
        var key = config["Jwt:Key"]!;
        var handler = new JwtSecurityTokenHandler();

        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateIssuer           = true,
            ValidIssuer              = config["Jwt:Issuer"],
            ValidateAudience         = true,
            ValidAudience            = config["Jwt:Audience"],
            ValidateLifetime         = false  // expired token is expected here
        };

        try
        {
            var principal = handler.ValidateToken(accessToken, parameters, out _);
            var idClaim   = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(idClaim, out var id) ? id : null;
        }
        catch
        {
            return null;
        }
    }
}