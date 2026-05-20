using FluentValidation;

namespace ClearFX.Application.Features.Wallets.Commands;

public class CreateWalletCommandValidator : AbstractValidator<CreateWalletCommand>
{
    private static readonly HashSet<string> AllowedCurrencies =
    [
        "USD", "EUR", "GBP", "EGP", "SAR", "AED", "KWD", "QAR", "BHD", "OMR"
    ];

    public CreateWalletCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency must be a 3-letter ISO code.")
            .Must(c => AllowedCurrencies.Contains(c.ToUpper()))
            .WithMessage($"Supported currencies: {string.Join(", ", AllowedCurrencies)}");
    }
}