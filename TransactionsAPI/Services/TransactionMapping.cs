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
            TransactionDate = TimeZoneInfo.ConvertTime(transaction.TransactionDate, transaction.TimeZone),
            ClientLocation = transaction.ClientLocation,
        };
    }

    public static TransactionsInfoModel CreateModelFromTransactionByCurrentUserTimeZone(this TransactionsInfo transaction, TimeZoneInfo timeZoneInfo) 
        => new ()
        {
            TransactionId = transaction.TransactionId,
            Name = transaction.Name,
            Email = transaction.Email,
            Amount = $"${transaction.Amount}",
            TransactionDate = TimeZoneInfo.ConvertTime(transaction.TransactionDate, timeZoneInfo),
            ClientLocation = transaction.ClientLocation,
        };

    public static TransactionsInfo CreateOriginTransactionFromModel(this TransactionsInfoModel model)
    {
        var amount = decimal.Parse(model.Amount.Substring(1));
        var timeZone = TimeZoneService.ConvertToTimeZoneInfo(model.ClientLocation);

        return new TransactionsInfo()
        {
            TransactionId = model.TransactionId,
            Name = model.Name,
            Email = model.Email,
            Amount = amount,
            TransactionDate = model.TransactionDate.UtcDateTime,
            ClientLocation = model.ClientLocation,
            TimeZone = timeZone,
        };
    }
}