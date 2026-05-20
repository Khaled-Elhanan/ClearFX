using MediatR;

namespace ClearFX.Application.Features.Wallets.Queries;

public record GetWalletQuery(Guid WalletId) : IRequest<WalletDto>;

public record WalletDto(
    Guid Id,
    Guid CustomerId,
    string Currency,
    decimal Balance,
    bool IsActive,
    DateTimeOffset CreatedAt
);