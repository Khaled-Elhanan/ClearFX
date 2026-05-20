using ClearFX.Domain.Entities;
using ClearFX.Domain.Interfaces;
using MediatR;

namespace ClearFX.Application.Features.Wallets.Queries;

public class GetCustomerWalletsQueryHandler(IRepository<Wallet> walletRepo)
    : IRequestHandler<GetCustomerWalletsQuery, IEnumerable<WalletDto>>
{
    public async Task<IEnumerable<WalletDto>> Handle(GetCustomerWalletsQuery request, CancellationToken cancellationToken)
    {
        var wallets = await walletRepo.FindAsync(
            w => w.CustomerId == request.CustomerId,
            cancellationToken);

        return wallets.Select(w => new WalletDto(
            w.Id, w.CustomerId, w.Currency, w.Balance, w.IsActive, w.CreatedAt
        ));
    }
}