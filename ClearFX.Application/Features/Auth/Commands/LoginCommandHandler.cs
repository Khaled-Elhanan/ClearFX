using ClearFX.Application.Common;
using ClearFX.Domain.Entities;
using ClearFX.Domain.Interfaces;
using MediatR;

namespace ClearFX.Application.Features.Auth.Commands;

public class LoginCommandHandler(IRepository<User> userRepo, IUnitOfWork uow, IJwtService jwtService)
    : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var users = await userRepo.FindAsync(u => u.Email == request.Email, cancellationToken);
        var user  = users.FirstOrDefault();

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is deactivated.");

        var (accessToken, refreshToken) = jwtService.GenerateTokens(user);

        user.RefreshToken = RefreshTokenCommandHandler.HashRefreshToken(refreshToken);

        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        user.LastLoginAt        = DateTimeOffset.UtcNow;

        await  uow.SaveChangesAsync(cancellationToken);

        return new LoginResult(accessToken, refreshToken, user.Role.ToString(), 3600);
    }
}