using ClearFX.Application.Common;
using ClearFX.Domain.Entities;
using ClearFX.Domain.Enums;
using ClearFX.Domain.Interfaces;
using MediatR;

namespace ClearFX.Application.Features.ExchangeRates.Commands;

public class SetExchangeRateCommandHandler(
    IRepository<ExchangeRate> rateRepo,
    IUnitOfWork uow,
    ICurrentUserService currentUser)
    : IRequestHandler<SetExchangeRateCommand, SetExchangeRateResult>
{
    public async Task<SetExchangeRateResult> Handle(SetExchangeRateCommand request, CancellationToken cancellationToken)
    {
        // 1. Deactivate existing active rate for this pair
        var existing = await rateRepo.FindAsync(
            r => r.FromCurrency == request.FromCurrency.ToUpper()
                 && r.ToCurrency   == request.ToCurrency.ToUpper()
                 && r.IsActive,
            cancellationToken);

        foreach (var old in existing)
        {
            old.IsActive = false;
            old.ValidTo  = DateTimeOffset.UtcNow;
            rateRepo.Update(old);
        }

        // 2. Create new active rate
        var rate = new ExchangeRate
        {
            FromCurrency = request.FromCurrency.ToUpper(),
            ToCurrency   = request.ToCurrency.ToUpper(),
            BuyRate      = request.BuyRate,
            SellRate     = request.SellRate,
            Source       = RateSource.Manual,
            IsActive     = true,
            SetBy        = currentUser.UserId
        };

        await rateRepo.AddAsync(rate, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return new SetExchangeRateResult(
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