using MediatR;

namespace ClearFX.Application.Features.Wallets.Commands;

public record CreateWalletCommand(
    Guid CustomerId,
    string Currency
) : IRequest<CreateWalletResult>;

public record CreateWalletResult(Guid WalletId, Guid CustomerId, string Currency, decimal Balance);