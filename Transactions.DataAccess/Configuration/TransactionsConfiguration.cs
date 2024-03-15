using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transactions.DataAccess.Entities;

namespace Transactions.DataAccess.Configuration;

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
            .Property(x => x.TransactionDate)
            .HasColumnType("datetime");

        builder
            .Property(e => e.ClientLocation)
            .HasConversion(
                v => $"{v.Latitude},{v.Longitude}",
                v => FromStringToLocation(v)
            );
    }

    private static Location FromStringToLocation(string location) 
        => new () {
            Latitude = double.Parse(location.Split(",")[0]),
            Longitude = double.Parse(location.Split(',')[1])
        };
}

