using ClearFX.Domain.Entities;
using ClearFX.Domain.Interfaces;
using MediatR;

namespace ClearFX.Application.Features.ExchangeRates.Queries;

public class GetAllRatesQueryHandler(IRepository<ExchangeRate> rateRepo)
    : IRequestHandler<GetAllRatesQuery, IEnumerable<ExchangeRateDto>>
{
    public async Task<IEnumerable<ExchangeRateDto>> Handle(GetAllRatesQuery request, CancellationToken cancellationToken)
    {
        var rates = request.ActiveOnly
            ? await rateRepo.FindAsync(r => r.IsActive, cancellationToken)
            : await rateRepo.GetAllAsync(cancellationToken);

        return rates.Select(r => new ExchangeRateDto(
            r.Id, r.FromCurrency, r.ToCurrency,
            r.BuyRate, r.SellRate, r.Spread, r.ValidFrom
        ));
    }
}