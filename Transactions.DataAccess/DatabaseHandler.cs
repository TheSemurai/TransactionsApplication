using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Transactions.DataAccess.Entities;
using TransactionsAPI.Infrastructure;

namespace Transactions.DataAccess;

public class DatabaseHandler
{
    private readonly IConfiguration _configuration;

    public DatabaseHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<IEnumerable<TransactionsInfo>> GetSpecificTransactionsByTimeZone(TimeZoneInfo timeZone, DateTimeOffset from,
        DateTimeOffset to)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var selectQuery = @"SELECT [TransactionId]
                                  ,[Name]
                                  ,[Email]
                                  ,[Amount]
                                  ,[TransactionDate]
                                  ,[ClientLocation]
                                  ,[TimeZone]
                              FROM [TransactionsDB].[dbo].[Transactions]
                              WHERE [TimeZone] = @timeZone 
                              AND [TransactionDate] BETWEEN @fromDate AND @toDate";
        var parameters = new
        {
            timeZone = timeZone.Id,
            fromDate = from,
            toDate = to,
        };
        
        await using var connection = new SqlConnection(connectionString);
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
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById(x.TimeZone),
        });
        
        connection.Close();
        return transactions;
    }
    
    public async Task<IEnumerable<TransactionsInfo>> GetCurrentTransactions(String email, DateTimeOffset from,
        DateTimeOffset to)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var selectQuery = @"SELECT [TransactionId]
                                  ,[Name]
                                  ,[Email]
                                  ,[Amount]
                                  ,[TransactionDate]
                                  ,[ClientLocation]
                              FROM [TransactionsDB].[dbo].[Transactions]
                              WHERE [Email] = @email 
                              AND [TransactionDate] BETWEEN @fromDate AND @toDate";
                              //AND [TimeZone] = @TimeZone"; // todo: delete
        var parameters = new
        {
            email = email,
            fromDate = from,
            toDate = to,
        };
        
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var allTransactions = await connection.QueryAsync<TransactionsInfo>(selectQuery, parameters);
        
        connection.Close();
        return allTransactions;
    }

    public async Task<IEnumerable<TransactionsInfo>> GetAllTransactions()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var selectQuery = @"SELECT [TransactionId]
                                  ,[Name]
                                  ,[Email]
                                  ,[Amount]
                                  ,[TransactionDate]
                                  ,[ClientLocation]
                                  ,[TimeZone]
                              FROM [TransactionsDB].[dbo].[Transactions]";
        
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        //var allTransactions = await connection.QueryAsync<TransactionsInfo>(selectQuery);
        
        var getAllRequest = await connection.QueryAsync(selectQuery);

        IEnumerable<TransactionsInfo> transactions = getAllRequest.Select(x => new TransactionsInfo()
        {
            TransactionId = x.TransactionId,
            Name = x.Name,
            Email = x.Email,
            Amount = x.Amount,
            TransactionDate = x.TransactionDate,
            ClientLocation = x.ClientLocation,
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById(x.TimeZone),
        });
        
        connection.Close();
        return transactions;
    }

    public async Task<RequestResult> InsertTransactionsAsync(ICollection<TransactionsInfo> transactions)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var insertQuery = @"MERGE [TransactionsDB].[dbo].[Transactions] WITH (SERIALIZABLE) AS OriginTrans
                            USING (VALUES (@TransactionId,@Name,@Email,@Amount,@TransactionDate,@ClientLocation,@TimeZone)) 
                                        AS Trans (TransactionId, Name, Email, Amount, TransactionDate, ClientLocation, Timezone)
                            ON Trans.TransactionId = OriginTrans.TransactionId
                            WHEN MATCHED THEN
                            UPDATE SET  OriginTrans.Name = Trans.Name, 
			                            OriginTrans.Email = Trans.Email,
			                            OriginTrans.Amount = Trans.Amount,
			                            OriginTrans.TransactionDate = Trans.TransactionDate,
			                            OriginTrans.ClientLocation = Trans.ClientLocation,
			                            OriginTrans.Timezone = Trans.Timezone
                            WHEN NOT MATCHED THEN
                            INSERT (TransactionId, Name, Email, Amount, TransactionDate, ClientLocation, Timezone)
                            VALUES (Trans.TransactionId, Trans.Name, Trans.Email, Trans.Amount, Trans.TransactionDate, Trans.ClientLocation, Trans.Timezone);";
        var parameters = transactions.Select(x => new
        {
            TransactionId = x.TransactionId, 
            Name = x.Name, 
            Email = x.Email, 
            Amount = x.Amount, 
            TransactionDate = x.TransactionDate, 
            ClientLocation = x.ClientLocation,
            Timezone = x.TimeZone.Id,
        });

        await using var connection = new SqlConnection(connectionString);
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