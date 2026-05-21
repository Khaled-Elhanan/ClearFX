namespace ClearFX.Application.Features.ExchangeRates;

public interface IRateSyncService
{
    Task SyncRateAsync(string fromCurrency, string toCurrency, CancellationToken cancellationToken = default);
    Task SyncAllRatesAsync(CancellationToken cancellationToken = default);
}