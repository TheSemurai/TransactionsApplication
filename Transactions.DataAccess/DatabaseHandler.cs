using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Transactions.DataAccess.Entities;
using TransactionsAPI.Services;

namespace Transactions.DataAccess;

/// <summary>
/// Class for handling requests to database
/// </summary>
public class DatabaseHandler
{
    private readonly string _connectionString;
    public DatabaseHandler(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    /// <summary>
    /// Getting a specific transactions by time zone
    /// </summary>
    /// <param name="timeZone">Specific time zone location</param>
    /// <param name="from">Specific from date</param>
    /// <param name="to">Specific to date</param>
    /// <returns></returns>
    public async Task<IEnumerable<TransactionsInfo>> GetSpecificTransactionsByTimeZone(TimeZoneInfo timeZone, DateTimeOffset from,
        DateTimeOffset to)
    {
        var selectQuery = @"SELECT [TransactionId]
                                  ,[Name]
                                  ,[Email]
                                  ,[Amount]
                                  ,[TransactionDate]
                                  ,[ClientLocation]
                                  ,[TimeZone]
                              FROM [TransactionsDB].[dbo].[Transactions]
                              WHERE [TransactionDate] BETWEEN @fromDate AND @toDate";
        var parameters = new
        {
            fromDate = TimeZoneInfo.ConvertTimeToUtc(from.DateTime, timeZone),
            toDate = TimeZoneInfo.ConvertTimeToUtc(to.DateTime, timeZone),
        };
        
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var getAllRequest = await connection.QueryAsync(selectQuery, parameters);

        IEnumerable<TransactionsInfo> transactions = getAllRequest.Select(x => new TransactionsInfo()
        {
            TransactionId = x.TransactionId,
            Name = x.Name,
            Email = x.Email,
            Amount = x.Amount,
            TransactionDate = x.TransactionDate,
            ClientLocation = x.ClientLocation,
            TimeZone = TimeZoneService.FindOrCreateTimeZoneById(x.TimeZone),
        });
        
        connection.Close();
        return transactions;
    }

    /// <summary>
    /// Getting all transaction from database
    /// </summary>
    /// <returns>IEnumerable of TransactionsInfo</returns>
    public async Task<IEnumerable<TransactionsInfo>> GetAllTransactions()
    {
        var selectQuery = @"SELECT [TransactionId]
                                  ,[Name]
                                  ,[Email]
                                  ,[Amount]
                                  ,[TransactionDate]
                                  ,[ClientLocation]
                                  ,[TimeZone]
                              FROM [TransactionsDB].[dbo].[Transactions]";
        
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var getAllRequest = await connection.QueryAsync(selectQuery);

        IEnumerable<TransactionsInfo> transactions = getAllRequest.Select(x => new TransactionsInfo()
        {
            TransactionId = x.TransactionId,
            Name = x.Name,
            Email = x.Email,
            Amount = x.Amount,
            TransactionDate = x.TransactionDate,
            ClientLocation = x.ClientLocation,
            TimeZone = TimeZoneService.FindOrCreateTimeZoneById(x.TimeZone),
        });
        
        connection.Close();
        return transactions;
    }

    /// <summary>
    /// Updating transaction by id, if it does not exist add to database
    /// </summary>
    /// <param name="transactions">Collection of transactions</param>
    /// <returns>Result of request</returns>
    public async Task<RequestResult> InsertTransactionsAsync(ICollection<TransactionsInfo> transactions)
    {
        var insertQuery = @"MERGE [TransactionsDB].[dbo].[Transactions] WITH (SERIALIZABLE) AS OriginTrans
                            USING (VALUES (@TransactionId,@Name,@Email,@Amount,@TransactionDate,@ClientLocation,@TimeZone, @TransactionDateAtLocal)) 
                                        AS Trans (TransactionId, Name, Email, Amount, TransactionDate, ClientLocation, Timezone, TransactionDateAtLocal)
                            ON Trans.TransactionId = OriginTrans.TransactionId
                            WHEN MATCHED THEN
                            UPDATE SET  OriginTrans.Name = Trans.Name, 
			                            OriginTrans.Email = Trans.Email,
			                            OriginTrans.Amount = Trans.Amount,
			                            OriginTrans.TransactionDate = Trans.TransactionDate,
			                            OriginTrans.ClientLocation = Trans.ClientLocation,
			                            OriginTrans.Timezone = Trans.Timezone,
			                            OriginTrans.TransactionDateAtLocal = Trans.TransactionDateAtLocal
                            WHEN NOT MATCHED THEN
                            INSERT (TransactionId, Name, Email, Amount, TransactionDate, ClientLocation, Timezone, TransactionDateAtLocal)
                            VALUES (Trans.TransactionId, Trans.Name, Trans.Email, Trans.Amount, Trans.TransactionDate, Trans.ClientLocation, Trans.Timezone, Trans.TransactionDateAtLocal);";
        var parameters = transactions.Select(x => new
        {
            TransactionId = x.TransactionId, 
            Name = x.Name, 
            Email = x.Email, 
            Amount = x.Amount, 
            TransactionDate = x.TransactionDate, 
            TransactionDateAtLocal = x.TransactionDateAtLocal,
            ClientLocation = x.ClientLocation,
            Timezone = x.TimeZone.Id,
        });

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = connection.BeginTransaction();
        
        try
        {
            var request = await connection.ExecuteAsync(insertQuery, parameters, transaction);

            if (request <= 0)
                return new RequestResult()
                {
                    Success = false,
                    Messages = new List<string>
                    {
                        "Something went wrong when trying to add your transactions.",
                    },
                };

            transaction.Commit();
            connection.Close();
            return new RequestResult()
            {
                Success = true,
                Messages = new List<string>
                {
                    $"Transactions was added successfully.",
                },
            };;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            
            transaction.Rollback();
            connection.Close();
            return new RequestResult()
            {
                Success = false,
                Messages = new List<string>
                {
                    $"Something went wrong"
                },
            };
        }
    }
}