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

    public async Task<RequestResult> InsertTransaction(TransactionsInfo transactionInfo)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        
        var insertQuery = @"INSERT INTO [TransactionsDB].[dbo].[Transactions] (TransactionId, Name, Email, Amount, TransactionDate, ClientLocation)
                            VALUES (@TransactionId,@Name,@Email,@Amount,@TransactionDate,@ClientLocation);";
        var parameters = new {
            TransactionId = transactionInfo.TransactionId,
            Name = transactionInfo.Name,
            Email = transactionInfo.Email,
            Amount = transactionInfo.Amount,
            TransactionDate = transactionInfo.TransactionDate,
            ClientLocation = $"{transactionInfo.ClientLocation.Latitude}, {transactionInfo.ClientLocation.Longitude}"
        };

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
                        $"Transaction {parameters.TransactionId} can not be added.",
                    },
                };
            
            #region MyRegion
            // var sqlCmd = new SqlCommand(insertQuery, connection, transaction);

            // sqlCmd.Parameters.AddWithValue("@TransactionId", transactionInfo.TransactionId);
            // sqlCmd.Parameters.AddWithValue("@Name", transactionInfo.Name);
            // sqlCmd.Parameters.AddWithValue("@Email", transactionInfo.Email);
            // sqlCmd.Parameters.AddWithValue("@Amount", transactionInfo.Amount);
            // sqlCmd.Parameters.AddWithValue("@TransactionDate", transactionInfo.TransactionDate);
            // sqlCmd.Parameters.AddWithValue("@ClientLocation", $"{transactionInfo.ClientLocation.Latitude}, {transactionInfo.ClientLocation.Longitude}");

            // sqlCmd.ExecuteNonQuery();
            #endregion
            
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