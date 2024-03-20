using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transactions.DataAccess.Entities;

namespace Transactions.DataAccess.Configuration;

/// <summary>
/// Configuration for making a migration data
/// </summary>
public class TransactionsConfiguration : IEntityTypeConfiguration<TransactionsInfo>
{
    public void Configure(EntityTypeBuilder<TransactionsInfo> builder)
    {
        builder
            .HasKey(x => x.TransactionId);

        builder
            .Property(x => x.Name)
            .HasMaxLength(25)
            .IsRequired();

        builder
            .Property(x => x.Email)
            .HasMaxLength(60)
            .IsRequired();

        builder
            .Property(x => x.Amount)
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0.00); //todo: delete it later mb

        builder
            .Property(x => x.TransactionDate);

        builder
            .Property(e => e.ClientLocation);

        builder
            .Property(x => x.TimeZone)
            .HasColumnType("nvarchar(MAX)")
            .HasConversion(
                v => v.Id,
                v => TimeZoneInfo.FindSystemTimeZoneById(v)
            );

    }
}

