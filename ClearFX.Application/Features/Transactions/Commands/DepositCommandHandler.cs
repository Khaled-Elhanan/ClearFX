using ClearFX.Application.Common;
using ClearFX.Domain.Entities;
using ClearFX.Domain.Enums;
using ClearFX.Domain.Interfaces;
using MediatR;

namespace ClearFX.Application.Features.Transactions.Commands;

public class DepositCommandHandler(
    IRepository<Wallet> walletRepo,
    IRepository<Transaction> transactionRepo,
    IUnitOfWork uow,
    ICurrentUserService currentUser)
    : IRequestHandler<DepositCommand, TransactionResult>
{
    public async Task<TransactionResult> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        var existing = await transactionRepo.FindAsync(
            t => t.IdempotencyKey == request.IdempotencyKey, cancellationToken);
        if (existing.Any())
        {
            var dup = existing.First();
            return new TransactionResult(dup.Id, dup.Type.ToString(),
                dup.Status.ToString(), dup.Amount, dup.Currency, dup.Reference, dup.CreatedAt);
        }

        var wallet = await walletRepo.GetByIdAsync(request.WalletId, cancellationToken)
            ?? throw new KeyNotFoundException($"Wallet '{request.WalletId}' not found.");

        if (!wallet.IsActive)
            throw new InvalidOperationException("Wallet is inactive.");

        var reference = $"DEP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        var transaction = new Transaction
        {
            Type           = TransactionType.Deposit,
            Status         = TransactionStatus.Pending,
            WalletId       = wallet.Id,
            Amount         = request.Amount,
            Currency       = wallet.Currency,
            Reference      = reference,
            IdempotencyKey = request.IdempotencyKey,
            Notes          = request.Notes,
            CreatedBy      = currentUser.UserId
        };

        await transactionRepo.AddAsync(transaction, cancellationToken);
        wallet.Credit(request.Amount);
        walletRepo.Update(wallet);
        transaction.Status = TransactionStatus.Completed;
        await uow.SaveChangesAsync(cancellationToken);

        return new TransactionResult(transaction.Id, transaction.Type.ToString(),
            transaction.Status.ToString(), transaction.Amount,
            transaction.Currency, transaction.Reference, transaction.CreatedAt);
    }
}