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
            // .HasColumnType("nvarchar(max)")
            // .HasColumnType("nvarchar")
            .HasConversion(
                v => $"{v.Latitude},{v.Longitude}",
                v => FromStringToLocation(v)
            );
        
        // builder
        //     .Property(x => x.ClientLocation)
        //     // .HasColumnType("string")
        //     .HasConversion(
        //         fromLocation => fromLocation.ToString(),
        //         stringLocation => ConvertToLocation(stringLocation)); // todo: refactor it;
    }

    private static Location FromStringToLocation(string location)
    {
        return new Location
        {
            Latitude = double.Parse(location.Split(",")[0]),
            Longitude = double.Parse(location.Split(',')[1])
        };
    }
    
    private static Location ConvertToLocation(string locationString)
    {
        var parts = locationString.Split(',');
        if (parts.Length != 2)
        {
            throw new ArgumentException("Invalid location string format");
        }

        double latitude, longitude;
        if (!double.TryParse(parts[0], out latitude) || !double.TryParse(parts[1], out longitude))
        {
            throw new ArgumentException("Invalid latitude or longitude format");
        }

        return new Location { Latitude = latitude, Longitude = longitude };
    }
}

