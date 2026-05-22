using ClearFX.Application.Features.ExchangeRates;
using Microsoft.Extensions.Logging;

namespace ClearFX.Infrastructure.ExternalApis;

public class RateSyncJob(IRateSyncService rateSyncService, ILogger<RateSyncJob> logger)
{
    public async Task ExecuteAsync()
    {
        logger.LogInformation("Rate sync job started at {Time}", DateTimeOffset.UtcNow);
        try
        {
            await rateSyncService.SyncAllRatesAsync();
            logger.LogInformation("Rate sync job completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Rate sync job failed.");
        }
    }
}