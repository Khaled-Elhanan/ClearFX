using ClearFX.Application.Common;
using ClearFX.Domain.Entities;
using ClearFX.Domain.Interfaces;
using MediatR;

namespace ClearFX.Application.Features.Wallets.Commands;

public class CreateWalletCommandHandler(
    IRepository<Wallet> walletRepo,
    IRepository<Customer> customerRepo,
    IUnitOfWork uow,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateWalletCommand, CreateWalletResult>
{
    public async Task<CreateWalletResult> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
    {
        // 1. Customer exists
        var customer = await customerRepo.GetByIdAsync(request.CustomerId, cancellationToken)
                       ?? throw new KeyNotFoundException($"Customer '{request.CustomerId}' not found.");

        if (!customer.IsActive)
            throw new InvalidOperationException("Cannot create wallet for an inactive customer.");

        // 2. Wallet already exists for this currency
        var existing = await walletRepo.FindAsync(
            w => w.CustomerId == request.CustomerId && w.Currency == request.Currency.ToUpper(),
            cancellationToken);

        if (existing.Any())
            throw new InvalidOperationException($"Customer already has a {request.Currency.ToUpper()} wallet.");

        // 3. Create wallet
        var wallet = new Wallet
        {
            CustomerId = request.CustomerId,
            Currency   = request.Currency.ToUpper()
        };

        await walletRepo.AddAsync(wallet, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return new CreateWalletResult(wallet.Id, wallet.CustomerId, wallet.Currency, wallet.Balance);
    }
}