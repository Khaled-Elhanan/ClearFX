using ClearFX.Domain.Enums;
using MediatR;

namespace ClearFX.Application.Features.ExchangeRates.Commands;

public record SetExchangeRateCommand(
    string FromCurrency,
    string ToCurrency,
    decimal BuyRate,
    decimal SellRate
) : IRequest<SetExchangeRateResult>;

public record SetExchangeRateResult(
    Guid RateId,
    string FromCurrency,
    string ToCurrency,
    decimal BuyRate,
    decimal SellRate,
    decimal Spread,
    DateTimeOffset ValidFrom
);