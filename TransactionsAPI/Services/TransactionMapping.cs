using Transactions.DataAccess.Entities;
using Transactions.DataAccess.Service;
using TransactionsAPI.Entities;

namespace TransactionsAPI.Services;

/// <summary>
/// Class for mapping original TransactionInfo with TransactionModel
/// </summary>
public static class TransactionMapping
{
    /// <summary>
    /// Mapping original object to transaction model
    /// </summary>
    /// <param name="transaction">Specific transaction</param>
    /// <returns>Mapped TransactionModel object</returns>
    public static TransactionsInfoModel CreateModelFromTransaction(this TransactionsInfo transaction)
    {
        return new TransactionsInfoModel()
        {
            TransactionId = transaction.TransactionId,
            Name = transaction.Name,
            Email = transaction.Email,
            Amount = $"${transaction.Amount}",
            TransactionDate = TimeZoneInfo.ConvertTime(transaction.TransactionDate, transaction.TimeZone).DateTime,
            ClientLocation = transaction.ClientLocation,
        };
    }
    
    /// <summary>
    /// Mapping a specific TransactionsInfo to TransactionsInfoModel at local time of these transactions
    /// </summary>
    /// <param name="transaction">Specific transaction for mapping with model</param>
    /// <returns>A new TransactionsInfoModel object at local time zone time</returns>
    public static TransactionsInfoModel CreateTransactionModelAtLocalTime(this TransactionsInfo transaction) 
        => new ()
        {
            TransactionId = transaction.TransactionId,
            Name = transaction.Name,
            Email = transaction.Email,
            Amount = $"${transaction.Amount}",
            TransactionDate = transaction.TransactionDateAtLocal,
            ClientLocation = transaction.ClientLocation,
        };

    /// <summary>
    /// Mapping original object to transaction model by specific user`s time zone
    /// </summary>
    /// <param name="transaction">Specific transaction</param>
    /// <param name="timeZoneInfo">Specific user`s time zone</param>
    /// <returns>Mapped TransactionModel object</returns>
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

    /// <summary>
    /// Mapping a model with original transaction object
    /// </summary>
    /// <param name="model">Specific transaction model</param>
    /// <returns>Mapped original TransactionInfo object</returns>
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
            TransactionDate = model.TransactionDate, // already in UTC by default
            TransactionDateAtLocal = TimeZoneInfo.ConvertTime(model.TransactionDate, timeZone),
            ClientLocation = model.ClientLocation,
            TimeZone = timeZone,
        };
    }
}