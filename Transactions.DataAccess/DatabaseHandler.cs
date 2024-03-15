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
    
    public async Task<RequestResult> InsertTransactionsAsync(ICollection<TransactionsInfo> transactions)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        
        var insertQuery = @"INSERT INTO [TransactionsDB].[dbo].[Transactions] (TransactionId, Name, Email, Amount, TransactionDate, ClientLocation)
                            VALUES (@TransactionId,@Name,@Email,@Amount,@TransactionDate,@ClientLocation);";

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var transaction = connection.BeginTransaction();
        
        try
        {
            var request = await connection.ExecuteAsync(insertQuery, transactions, transaction);

            if (request <= 0)
                return new RequestResult()
                {
                    Success = false,
                    Messages = new List<string>
                    {
                        //$"Transaction {parameters.TransactionId} can not be added.",
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
    public async Task<RequestResult> InsertTransactionAsync(TransactionsInfo transactionInfo)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        
        var insertQuery = @"INSERT INTO [TransactionsDB].[dbo].[Transactions] (TransactionId, Name, Email, Amount, TransactionDate, ClientLocation)
                            VALUES (@TransactionId,@Name,@Email,@Amount,@TransactionDate,@ClientLocation);";

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var transaction = connection.BeginTransaction();
        
        try
        {
            //var request = await connection.ExecuteAsync(insertQuery, parameters, transaction);
            var request = await connection.ExecuteAsync(insertQuery, transactionInfo, transaction);
            
            if (request <= 0)
                return new RequestResult()
                {
                    Success = false,
                    Messages = new List<string>
                    {
                        $"Transaction {transactionInfo.TransactionId} can not be added.",
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
            };
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            
            transaction.Rollback();
            connection.Close();
            return new RequestResult()
            {
                Success = false,
                Messages = new List<string>
                {
                    $"Something went wrong",
                    exception.Message,
                },
            };
        }
    }
}