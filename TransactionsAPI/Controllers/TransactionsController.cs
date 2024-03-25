using Microsoft.AspNetCore.Mvc;
using Transactions.DataAccess;
using Transactions.DataAccess.Entities;
using Transactions.DataAccess.Service;
using TransactionsAPI.Entities;
using TransactionsAPI.Infrastructure;
using TransactionsAPI.Infrastructure.Interfaces;
using TransactionsAPI.Services;

namespace TransactionsAPI.Controllers;

public class TransactionsController : BaseController
{
    private readonly IParser<TransactionsInfoModel> _parserCsv;
    private readonly IWriter<TransactionsInfoModel> _writerExcel;
    private readonly DatabaseHandler _databaseHandler;

    public TransactionsController(
        IParser<TransactionsInfoModel> parserCsv, 
        IWriter<TransactionsInfoModel> writerExcel, 
        DatabaseHandler databaseHandler)
    {
        _parserCsv = parserCsv;
        _writerExcel = writerExcel;
        _databaseHandler = databaseHandler;
    }

    /// <summary>
    /// Getting transactions from specific range in current user time zone
    /// </summary>
    /// <param name="timeZoneId">Specific time zone id by IANA</param>
    /// <param name="from">From specific time</param>
    /// <param name="to">To specific time</param>
    /// <returns>List of transactions by specific range</returns>
    [HttpGet]
    [Route("FindTransactionByCurrentUserTimeZone")]
    public async Task<IActionResult> FindTransactionByCurrentUserTimeZone(string timeZoneId, DateTime from,
        DateTime to)
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
            model.CreateModelFromTransactionByCurrentUserTimeZone(timeZone));
        
        return Ok(transactionsModel);
    }
    
    /// <summary>
    /// Getting transactions from specific range at local user time
    /// </summary>
    /// <param name="from">From specific time</param>
    /// <param name="to">To specific time</param>
    /// <returns>List of transactions by specific range</returns>
    [HttpGet]
    [Route("FindTransactionInAnotherUserTimeZone")]
    public async Task<IActionResult> FindTransactionInAnotherUserTimeZone(DateTime from,
        DateTime to)
    {
        var transactions = await _databaseHandler.GetTransactionsByTheirTime( from, to);
        
        if (!transactions.Any())
            return NoContent();
        
        var transactionsModel = transactions.Select(model => 
            model.CreateTransactionModelAtLocalTime());

        return Ok(transactionsModel);
    }

    /// <summary>
    /// Importing a specific transactions data from csv file into database
    /// </summary>
    /// <param name="file">Specific data of transactions</param>
    /// <returns>Result of request</returns>
    [HttpPost]
    [Route("ImportExcelData")]
    public async Task<IActionResult> ImportExcelData(IFormFile file)
    {
        var transactionsModel = _parserCsv.ReadFromFile(file);

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
    /// Exporting transactions data to xlsx file by UTC time 
    /// </summary>
    /// <param name="exportedColumns">Chosen columns by user</param>
    /// <returns>Excel file (format: xlsx)</returns>
    [HttpPost]
    [Route("ExportToExcel")]
    public async Task<IActionResult> ExportToExcel(string timeZoneId, ExportedColumns exportedColumns)
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
        
        var transactions = await _databaseHandler.GetAllTransactions();

        if (!transactions.Any())
            return NoContent();
        
        var transactionsModel = transactions.Select(model => 
            model.CreateModelFromTransaction()).ToList();
        
        var fileInBytes = _writerExcel.WriteIntoFileWithCustomHeader(transactionsModel, exportedColumns);
        
        return File(
            fileInBytes, 
            "application/octet-stream", 
            $"exported_data_{TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local), timeZone).ToString("yyyy-MM-dd HH:mm:ss")}.xlsx"
            );
    }
}