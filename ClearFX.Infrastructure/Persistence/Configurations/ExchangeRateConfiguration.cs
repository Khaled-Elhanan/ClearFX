using ClearFX.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearFX.Infrastructure.Persistence.Configurations;

public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
{
    public void Configure(EntityTypeBuilder<ExchangeRate> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FromCurrency).IsRequired().HasMaxLength(3);
        builder.Property(x => x.ToCurrency).IsRequired().HasMaxLength(3);
        builder.Property(x => x.BuyRate).HasPrecision(18, 6);
        builder.Property(x => x.SellRate).HasPrecision(18, 6);
        builder.Property(x => x.Source).HasConversion<string>();
        builder.Ignore(x => x.Spread);

        builder.HasOne(x => x.SetByUser)
            .WithMany()
            .HasForeignKey(x => x.SetBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}