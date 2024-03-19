using GeoTimeZone;
using Transactions.DataAccess.Entities;
using TransactionsAPI.Entities;

namespace TransactionsAPI.Services;

public static class TransactionMapping
{
    public static TransactionsInfoModel CreateModelFromTransaction(this TransactionsInfo transaction)
    {
        return new TransactionsInfoModel()
        {
            TransactionId = transaction.TransactionId,
            Name = transaction.Name,
            Email = transaction.Email,
            Amount = $"${transaction.Amount}",
            TransactionDate = transaction.TransactionDate,
            ClientLocation = transaction.ClientLocation,
        };
    }
    
    public static TransactionsInfo CreateOriginTransactionFromModel(this TransactionsInfoModel model)
    {
        var amount = decimal.Parse(model.Amount.Substring(1));

        var coordinates = model.ClientLocation.Split(",");
        var latitude = double.Parse(coordinates[0]);
        var longitude = double.Parse(coordinates[1]);

        var timeZoneId = TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        
        return new TransactionsInfo()
        {
            TransactionId = model.TransactionId,
            Name = model.Name,
            Email = model.Email,
            Amount = amount,
            TransactionDate = model.TransactionDate,
            ClientLocation = model.ClientLocation,
            TimeZone = timeZone,
        };
    }
}