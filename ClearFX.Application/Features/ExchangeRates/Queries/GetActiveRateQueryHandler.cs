using ClearFX.Domain.Entities;
using ClearFX.Domain.Exceptions;
using ClearFX.Domain.Interfaces;
using MediatR;

namespace ClearFX.Application.Features.ExchangeRates.Queries;

public class GetActiveRateQueryHandler(IRepository<ExchangeRate> rateRepo)
    : IRequestHandler<GetActiveRateQuery, ExchangeRateDto>
{
    public async Task<ExchangeRateDto> Handle(GetActiveRateQuery request, CancellationToken cancellationToken)
    {
        var rates = await rateRepo.FindAsync(
            r => r.FromCurrency == request.FromCurrency.ToUpper()
                 && r.ToCurrency   == request.ToCurrency.ToUpper()
                 && r.IsActive,
            cancellationToken);

        var rate = rates.FirstOrDefault()
                   ?? throw new ExchangeRateNotFoundException(request.FromCurrency, request.ToCurrency);

        return new ExchangeRateDto(
            rate.Id,
            rate.FromCurrency,
            rate.ToCurrency,
            rate.BuyRate,
            rate.SellRate,
            rate.Spread,
            rate.ValidFrom
        );
    }
}