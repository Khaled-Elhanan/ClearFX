using MediatR;

namespace ClearFX.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;

public record LoginResult(string AccessToken, string RefreshToken, string Role, int ExpiresIn);