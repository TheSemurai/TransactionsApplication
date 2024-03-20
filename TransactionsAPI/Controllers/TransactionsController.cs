using GeoTimeZone;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
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

    [HttpGet]
    [Route("MyTransactions")]
    public async Task<IActionResult> MyTransactions(string email, DateTimeOffset from,
        DateTimeOffset to)
    {
        var transactions = await _databaseHandler.GetCurrentTransactions(email, from, to);

        if (!transactions.Any())
            return NoContent();

        var model = transactions.Select(transaction => 
            transaction.CreateModelFromTransaction());
        
        return Ok(model);
    }
    
    [HttpGet]
    [Route("AllTransactionsByTimeZone")]
    public async Task<IActionResult> AllTransactionsByTimeZone(string timeZoneId, DateTimeOffset from,
        DateTimeOffset to)
    {
        TimeZoneInfo timeZone;
        try
        {
            timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
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
            model.CreateModelFromTransaction()).ToList();
        
        return Ok(transactionsModel);
    }

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

    [HttpGet]
    [Route("ExportToExcel")]
    public async Task<IActionResult> ExportToExcel()
    {
        var transactions = (await _databaseHandler.GetAllTransactions()).ToList();

        if (!transactions.Any())
            return NoContent();
        
        var transactionsModel = transactions.Select(model => 
            model.CreateModelFromTransaction()).ToList();

        var fileInBytes = _parser.WriteIntoFile(transactionsModel);

        return File(fileInBytes, "application/octet-stream", $"exported_data.csv");
    }
    
    [HttpGet]
    [Route("ExportToExcelFromSpecificTimeZone")]
    public async Task<IActionResult> ExportToExcelFromSpecificTimeZone(string coordinates)
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
        
        var transactions = (await _databaseHandler.GetAllTransactions()).ToList();

        if (!transactions.Any())
            return NoContent();
        
        var transactionsModel = transactions.Select(model => 
            model.CreateModelFromTransactionByCurrentUserTimeZone(timeZoneInfo)).ToList();

        var fileInBytes = _parser.WriteIntoFile(transactionsModel);

        return File(fileInBytes, "application/octet-stream", $"exported_data_{TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo)}.csv");
    }
}