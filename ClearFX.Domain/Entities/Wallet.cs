using ClearFX.Domain.Exceptions;

namespace ClearFX.Domain.Entities;

public class Wallet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Nav
    public Customer? Customer { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    
    
    // Domain methods
    public void Debit(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Debit amount must be positive.");
        if (Balance < amount)
            throw new InsufficientBalanceException(Currency, Balance, amount);
        Balance -= amount;
    }

    public void Credit(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Credit amount must be positive.");
        Balance += amount;
    }
}