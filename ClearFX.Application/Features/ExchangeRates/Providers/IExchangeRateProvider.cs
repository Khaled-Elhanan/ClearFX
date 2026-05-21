namespace ClearFX.Application.Features.ExchangeRates.Providers;


public interface IExchangeRateProvider
{
    string ProviderName { get; }
    Task<ExchangeRateData> GetRateAsync(
        string fromCurrency,
        string toCurrency,
        CancellationToken cancellationToken = default);
}