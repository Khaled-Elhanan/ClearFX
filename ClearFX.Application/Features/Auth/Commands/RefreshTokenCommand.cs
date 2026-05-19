using MediatR;

namespace ClearFX.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<LoginResult>;