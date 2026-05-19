using ClearFX.Domain.Enums;
using MediatR;

namespace ClearFX.Application.Features.Auth.Commands;

public record RegisterCommand(
    string FullName,
    string Email,
    string Password,
    UserRole Role
) : IRequest<RegisterResult>;

public record RegisterResult(Guid UserId, string Email, string Role);