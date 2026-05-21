using FluentValidation;

namespace ClearFX.Application.Features.ExchangeRates.Commands;

public class SetExchangeRateCommandValidator : AbstractValidator<SetExchangeRateCommand>
{
    private static readonly HashSet<string> AllowedCurrencies =
    [
        "USD", "EUR", "GBP", "EGP", "SAR", "AED", "KWD", "QAR", "BHD", "OMR"
    ];

    public SetExchangeRateCommandValidator()
    {
        RuleFor(x => x.FromCurrency)
            .NotEmpty()
            .Length(3)
            .Must(c => AllowedCurrencies.Contains(c.ToUpper()))
            .WithMessage("Invalid source currency.");

        RuleFor(x => x.ToCurrency)
            .NotEmpty()
            .Length(3)
            .Must(c => AllowedCurrencies.Contains(c.ToUpper()))
            .WithMessage("Invalid target currency.");

        RuleFor(x => x)
            .Must(x => x.FromCurrency.ToUpper() != x.ToCurrency.ToUpper())
            .WithMessage("From and To currencies must be different.");

        RuleFor(x => x.BuyRate)
            .GreaterThan(0).WithMessage("Buy rate must be greater than zero.");

        RuleFor(x => x.SellRate)
            .GreaterThan(0).WithMessage("Sell rate must be greater than zero.");

        RuleFor(x => x)
            .Must(x => x.SellRate >= x.BuyRate)
            .WithMessage("Sell rate must be greater than or equal to buy rate.");
    }
}