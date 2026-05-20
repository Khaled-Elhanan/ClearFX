using ClearFX.Domain.Entities;
using ClearFX.Domain.Interfaces;
using MediatR;

namespace ClearFX.Application.Features.Wallets.Queries;

public class GetWalletQueryHandler(IRepository<Wallet> walletRepo)
    : IRequestHandler<GetWalletQuery, WalletDto>
{
    public async Task<WalletDto> Handle(GetWalletQuery request, CancellationToken cancellationToken)
    {
        var wallet = await walletRepo.GetByIdAsync(request.WalletId, cancellationToken)
                     ?? throw new KeyNotFoundException($"Wallet '{request.WalletId}' not found.");

        return new WalletDto(
            wallet.Id,
            wallet.CustomerId,
            wallet.Currency,
            wallet.Balance,
            wallet.IsActive,
            wallet.CreatedAt
        );
    }
}