namespace ClearFX.Domain.Exceptions;

public class WalletNotFoundException : DomainException
{
    public WalletNotFoundException(Guid walletId)
        : base($"Wallet with ID '{walletId}' was not found.") { }
}