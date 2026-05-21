using ClearFX.Domain.Enums;

namespace ClearFX.Domain.Entities;

public class ExchangeRate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
    public decimal BuyRate { get; set; }
    public decimal SellRate { get; set; }
    public decimal Spread => SellRate - BuyRate;
    public RateSource Source { get; set; }
    public string? ProviderName { get; set; }
    public string? ExternalReferenceId { get; set; }
    public DateTimeOffset? FetchedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset ValidFrom { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ValidTo { get; set; }
    public Guid? SetBy { get; set; }

    // Nav
    public User? SetByUser { get; set; }
}