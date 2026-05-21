using System.Text.Json;
using ClearFX.Application.Features.ExchangeRates.Providers;
using Microsoft.Extensions.Configuration;

namespace ClearFX.Infrastructure.ExternalApis;

public class ExternalExchangeRateProvider(
    HttpClient httpClient,
    IConfiguration config) : IExchangeRateProvider
{
    public string ProviderName => "ExchangeRate-API";

    public async Task<ExchangeRateData> GetRateAsync(
        string fromCurrency,
        string toCurrency,
        CancellationToken cancellationToken = default)
    {
        var apiKey  = config["ExchangeRateApi:ApiKey"]
                      ?? throw new InvalidOperationException("ExchangeRateApi:ApiKey not configured.");

        var url = $"https://v6.exchangerate-api.com/v6/{apiKey}/pair/{fromCurrency}/{toCurrency}";

        var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var doc  = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.GetProperty("result").GetString() != "success")
            throw new InvalidOperationException($"ExchangeRate-API error for {fromCurrency}/{toCurrency}.");

        var midRate = root.GetProperty("conversion_rate").GetDecimal();

        
        var spread  = 0.005m;
        var buyRate  = Math.Round(midRate * (1 - spread), 6);
        var sellRate = Math.Round(midRate * (1 + spread), 6);

        return new ExchangeRateData(
            FromCurrency:        fromCurrency.ToUpper(),
            ToCurrency:          toCurrency.ToUpper(),
            BuyRate:             buyRate,
            SellRate:            sellRate,
            ProviderName:        ProviderName,
            ExternalReferenceId: root.GetProperty("time_last_update_unix").ToString(),
            FetchedAt:           DateTimeOffset.UtcNow
        );
    }
}