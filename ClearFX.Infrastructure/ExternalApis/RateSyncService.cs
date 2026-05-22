using ClearFX.Application.Features.ExchangeRates;
using ClearFX.Application.Features.ExchangeRates.Providers;
using ClearFX.Domain.Entities;
using ClearFX.Domain.Enums;
using ClearFX.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClearFX.Infrastructure.ExternalApis;

public class RateSyncService(
    IExchangeRateProvider provider,
    IRepository<ExchangeRate> rateRepo,
    IUnitOfWork uow,
    ILogger<RateSyncService> logger) : IRateSyncService
{
    private static readonly (string From, string To)[] TrackedPairs =
    [
        ("USD", "EGP"), ("EUR", "EGP"), ("GBP", "EGP"),
        ("SAR", "EGP"), ("AED", "EGP"), ("USD", "EUR"),
        ("USD", "GBP"), ("EUR", "USD"), ("GBP", "USD"),
    ];

    public async Task SyncRateAsync(
        string fromCurrency,
        string toCurrency,
        CancellationToken cancellationToken = default)
    {
        // Fetch latest rate from provider
        var data = await provider.GetRateAsync(
            fromCurrency,
            toCurrency,
            cancellationToken);

        // Deactivate existing active rates
        var existing = await rateRepo.FindAsync(
            r => r.FromCurrency == fromCurrency.ToUpper()
              && r.ToCurrency   == toCurrency.ToUpper()
              && r.IsActive,
            cancellationToken);

        foreach (var old in existing)
        {
            old.IsActive = false;
            old.ValidTo  = DateTimeOffset.UtcNow;

            rateRepo.Update(old);
        }

        // Create new active rate snapshot
        var rate = new ExchangeRate
        {
            FromCurrency        = data.FromCurrency,
            ToCurrency          = data.ToCurrency,
            BuyRate             = data.BuyRate,
            SellRate            = data.SellRate,
            Source              = RateSource.ExchangeRateAPI,
            ProviderName        = data.ProviderName,
            ExternalReferenceId = data.ExternalReferenceId,
            FetchedAt           = data.FetchedAt,
            IsActive            = true,
            SetBy               = null
        };

        await rateRepo.AddAsync(rate, cancellationToken);

        await uow.SaveChangesAsync(cancellationToken);
    }

    public async Task SyncAllRatesAsync(
        CancellationToken cancellationToken = default)
    {
        foreach (var (from, to) in TrackedPairs)
        {
            try
            {
                await SyncRateAsync(from, to, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to sync rate {FromCurrency}/{ToCurrency}",
                    from,
                    to);
            }
        }
    }
}