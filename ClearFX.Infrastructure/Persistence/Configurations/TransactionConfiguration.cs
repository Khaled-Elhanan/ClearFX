using ClearFX.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearFX.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasConversion<string>();
        builder.Property(x => x.Status).HasConversion<string>();
        builder.Property(x => x.Amount).HasPrecision(18, 4);
        builder.Property(x => x.TargetAmount).HasPrecision(18, 4);
        builder.Property(x => x.RateApplied).HasPrecision(18, 6);
        builder.Property(x => x.Fee).HasPrecision(18, 4);
        builder.Property(x => x.Currency).IsRequired().HasMaxLength(3);
        builder.Property(x => x.Reference).IsRequired().HasMaxLength(50);
        builder.Property(x => x.IdempotencyKey).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.IdempotencyKey).IsUnique();

        builder.HasOne(x => x.Wallet)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.WalletId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TargetWallet)
            .WithMany()
            .HasForeignKey(x => x.TargetWalletId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}