using MediatR;

namespace ClearFX.Application.Features.ExchangeRates.Queries;

public record GetAllRatesQuery(bool ActiveOnly = true) : IRequest<IEnumerable<ExchangeRateDto>>;