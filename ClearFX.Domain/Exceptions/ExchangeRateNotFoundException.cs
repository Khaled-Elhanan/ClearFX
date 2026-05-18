namespace ClearFX.Domain.Exceptions;

public class ExchangeRateNotFoundException : DomainException
{
    public ExchangeRateNotFoundException(string from, string to)
        : base($"No active exchange rate found for {from} → {to}.") { }
}