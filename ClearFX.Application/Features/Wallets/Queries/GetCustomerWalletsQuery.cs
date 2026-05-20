using MediatR;

namespace ClearFX.Application.Features.Wallets.Queries;

public record GetCustomerWalletsQuery(Guid CustomerId) : IRequest<IEnumerable<WalletDto>>;