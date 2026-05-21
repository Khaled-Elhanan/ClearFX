using MediatR;

namespace ClearFX.Application.Features.ExchangeRates.Queries;

public record GetActiveRateQuery(string FromCurrency, string ToCurrency) : IRequest<ExchangeRateDto>;

public record ExchangeRateDto(
    Guid Id,
    string FromCurrency,
    string ToCurrency,
    decimal BuyRate,
    decimal SellRate,
    decimal Spread,
    DateTimeOffset ValidFrom
);