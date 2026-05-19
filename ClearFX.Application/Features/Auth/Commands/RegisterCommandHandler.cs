using ClearFX.Domain.Entities;
using ClearFX.Domain.Interfaces;
using MediatR;

namespace ClearFX.Application.Features.Auth.Commands;

public class RegisterCommandHandler(IRepository<User> userRepo, IUnitOfWork uow)
    : IRequestHandler<RegisterCommand, RegisterResult>
{
    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await userRepo.FindAsync(u => u.Email == request.Email, cancellationToken);
        if (existing.Any())
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            FullName     = request.FullName,
            Email        = request.Email,
            Role         = request.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await userRepo.AddAsync(user, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return new RegisterResult(user.Id, user.Email, user.Role.ToString());
    }
}