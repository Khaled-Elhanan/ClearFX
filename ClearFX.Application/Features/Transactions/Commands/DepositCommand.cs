using MediatR;

namespace ClearFX.Application.Features.Transactions.Commands;

public record DepositCommand(
    Guid WalletId,
    decimal Amount,
    string? Notes,
    string IdempotencyKey
) : IRequest<TransactionResult>;
public record TransactionResult(
    Guid TransactionId,
    string Type,
    string Status,
    decimal Amount,
    string Currency,
    string Reference,
    DateTimeOffset CreatedAt
);