using ClearFX.Application.Features.ExchangeRates.Providers;

namespace ClearFX.Infrastructure.ExternalApis;

public class ManualExchangeRateProvider(decimal buyRate, decimal sellRate) : IExchangeRateProvider
{
    public string ProviderName => "Manual";

    public Task<ExchangeRateData> GetRateAsync(
        string fromCurrency,
        string toCurrency,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ExchangeRateData(
            FromCurrency:        fromCurrency.ToUpper(),
            ToCurrency:          toCurrency.ToUpper(),
            BuyRate:             buyRate,
            SellRate:            sellRate,
            ProviderName:        ProviderName,
            ExternalReferenceId: null,
            FetchedAt:           DateTimeOffset.UtcNow
        ));
    }
}