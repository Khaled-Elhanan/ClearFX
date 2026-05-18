namespace ClearFX.Domain.Exceptions;

public class InsufficientBalanceException :DomainException
{
    public InsufficientBalanceException(string currency, decimal available, decimal requested)
        : base($"Insufficient balance in {currency} wallet. Available: {available}, Requested: {requested}") { }

}