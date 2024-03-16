using Microsoft.AspNetCore.Mvc;
using Transactions.DataAccess;
using Transactions.DataAccess.Entities;
using TransactionsAPI.Infrastructure;
using TransactionsAPI.Infrastructure.Interfaces;

namespace TransactionsAPI.Controllers;

public class TransactionsController : BaseController
{
    private readonly IParser<TransactionsInfo> _parser;
    private readonly DatabaseHandler _databaseHandler;

    public TransactionsController(
        IParser<TransactionsInfo> parser, 
        DatabaseHandler databaseHandler)
    {
        _parser = parser;
        _databaseHandler = databaseHandler;
    }

    [HttpPost]
    [Route("ImportExcelData")]
    public async Task<IActionResult> ImportExcelData(IFormFile file)
    {
        var transactions = _parser.ReadFromFile(file);

        if (!transactions.Any())
            NoContent();

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

        var fileInBytes = _parser.WriteIntoFile(transactions);

        return File(fileInBytes, "application/octet-stream", "exported_data.csv");
    }
}