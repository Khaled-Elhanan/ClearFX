using ClearFX.Domain.Enums;

namespace ClearFX.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public Guid WalletId { get; set; }
    public Guid? TargetWalletId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal? TargetAmount { get; set; }
    public string? TargetCurrency { get; set; }
    public Guid? ExchangeRateId { get; set; }
    public decimal? RateApplied { get; set; }
    public decimal Fee { get; set; } = 0;
    public string Reference { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Nav
    public Wallet? Wallet { get; set; }
    public Wallet? TargetWallet { get; set; }
    public ExchangeRate? ExchangeRate { get; set; }
    public User? CreatedByUser { get; set; }
}