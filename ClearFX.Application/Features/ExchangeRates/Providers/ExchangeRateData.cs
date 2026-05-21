namespace ClearFX.Application.Features.ExchangeRates.Providers;

public record ExchangeRateData(
    string FromCurrency,
    string ToCurrency,
    decimal BuyRate,
    decimal SellRate,
    string ProviderName,
    string? ExternalReferenceId,
    DateTimeOffset FetchedAt
);