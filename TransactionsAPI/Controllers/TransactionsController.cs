using Microsoft.AspNetCore.Mvc;
using Transactions.DataAccess;
using Transactions.DataAccess.Entities;
using TransactionsAPI.Entities;
using TransactionsAPI.Infrastructure;
using TransactionsAPI.Infrastructure.Interfaces;
using TransactionsAPI.Services;

namespace TransactionsAPI.Controllers;

public class TransactionsController : BaseController
{
    private readonly IParser<TransactionsInfoModel> _parser;
    private readonly DatabaseHandler _databaseHandler;

    public TransactionsController(
        IParser<TransactionsInfoModel> parser,
        DatabaseHandler databaseHandler)
    {
        _parser = parser;
        _databaseHandler = databaseHandler;
    }

    /// <summary>
    /// Getting all transactions by specific range (from, to) based on time zones other users
    /// </summary>
    /// <param name="timeZoneId">Specific data </param>
    /// <param name="from">Search from date</param>
    /// <param name="to">Search to date</param>
    /// <returns>Transaction by specific time zone</returns>
    [HttpGet]
    [Route("AllTransactionsByTimeZone")]
    public async Task<IActionResult> AllTransactionsByTimeZone(string timeZoneId, DateTimeOffset from,
        DateTimeOffset to)
    {
        TimeZoneInfo timeZone;
        try
        {
            timeZone = TimeZoneService.FindOrCreateTimeZoneById(timeZoneId);
        }
        catch (Exception e)
        {
            return BadRequest(new RequestResult()
            {
                Success = false,
                Messages = new List<string>()
                {
                    $"Something went wrong with time zone name (name: {timeZoneId}).",
                }
            });
        }

        var transactions = await _databaseHandler.GetSpecificTransactionsByTimeZone(timeZone, from, to);
        
        if (!transactions.Any())
             return NoContent();
        
        var transactionsModel = transactions.Select(model => 
            model.CreateModelFromTransaction());
        
        return Ok(transactionsModel);
    }

    /// <summary>
    /// Importing a specific data from file to database
    /// </summary>
    /// <param name="file">Specific data of transactions</param>
    /// <returns>Result of request</returns>
    [HttpPost]
    [Route("ImportExcelData")]
    public async Task<IActionResult> ImportExcelData(IFormFile file)
    {
        var transactionsModel = _parser.ReadFromFile(file);

        if (!transactionsModel.Any())
            return NoContent();

        IList<TransactionsInfo> transactions;

        try
        {
            transactions = transactionsModel.Select(model => 
                model.CreateOriginTransactionFromModel()).ToList();
        }
        catch (Exception e)
        {
            return BadRequest(new RequestResult()
            {
                Success = false,
                Messages = new List<string>()
                {
                    $"Something went wrong with transactions.",
                }
            });
        }

        var request = await _databaseHandler.InsertTransactionsAsync(transactions);

        if (request.Success)
            return Ok(request);

        return BadRequest(request);
    }

    /// <summary>
    /// Exporting data from database without specific user`s time zone
    /// </summary>
    /// <param name="exportedColumns">Chosen columns by user</param>
    /// <returns>Excel file (csv)</returns>
    [HttpPost]
    [Route("ExportToExcel")]
    public async Task<IActionResult> ExportToExcel(ExportedColumns exportedColumns)
    {
        var transactions = await _databaseHandler.GetAllTransactions();

        if (!transactions.Any())
            return NoContent();
        
        var transactionsModel = transactions.Select(model => 
            model.CreateModelFromTransaction()).ToList();
        
        var fileInBytes = _parser.WriteIntoFileWithCustomHeader(transactionsModel, exportedColumns);
        
        return File(fileInBytes, "application/octet-stream", $"exported_data.csv");
    }
    
    /// <summary>
    /// Exporting data from database with time by specific user`s time zone
    /// </summary>
    /// <param name="coordinates">Specific user coordinates</param>
    /// <param name="exportedColumns">Chosen columns by user</param>
    /// <returns>Excel file (csv)</returns>
    [HttpPost]
    [Route("ExportToExcelFromSpecificTimeZone")]
    public async Task<IActionResult> ExportToExcelFromSpecificTimeZone(string coordinates, ExportedColumns exportedColumns)
    {
        TimeZoneInfo timeZoneInfo;
        try
        {
            timeZoneInfo = TimeZoneService.ConvertToTimeZoneInfo(coordinates);
        }
        catch (Exception e)
        {
            return BadRequest("Something went wrong by coordinates.");
        }
        
        var transactions = (await _databaseHandler.GetAllTransactions());

        if (!transactions.Any())
            return NoContent();
        
        var transactionsModel = transactions.Select(model => 
            model.CreateModelFromTransactionByCurrentUserTimeZone(timeZoneInfo)).ToList();
        
        var fileInBytes = _parser.WriteIntoFileWithCustomHeader(transactionsModel, exportedColumns);
        
        return File(fileInBytes, "application/octet-stream", $"exported_data_{TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo)}.csv");
    }
}