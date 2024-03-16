using Microsoft.AspNetCore.Mvc;
using Transactions.DataAccess;
using Transactions.DataAccess.Entities;
using TransactionsAPI.Infrastructure;
using TransactionsAPI.Infrastructure.Interfaces;

namespace TransactionsAPI.Controllers;

public class TransactionsController : BaseController
{
    private readonly ApplicationContext _dbContext;
    private readonly IParser<TransactionsInfo> _parser;
    private readonly DatabaseHandler _databaseHandler;

    public TransactionsController(
        ApplicationContext dbContext, 
        IParser<TransactionsInfo> parser, 
        DatabaseHandler databaseHandler)
    {
        _dbContext = dbContext;
        _parser = parser;
        _databaseHandler = databaseHandler;
    }

    [HttpGet]
    [Route("GetString")]
    public async Task<IActionResult> GetString()
    {
        return Ok(new RequestResult()
        {
            Success = true,
            Messages = new List<string>()
            {
                "actually works",
            },
        });
    }
    
    [HttpPost]
    [Route("InsertTestTransaction")]
    public async Task<IActionResult> InsertTestTransaction()
    {
        var trans = new TransactionsInfo()
        {
            TransactionId = "lol54565646",
            Email = "a@a.com",
            Name = "Ilya Pupkin",
            Amount = 218.44563m,
            TransactionDate = new DateTime(2001,12,12,12,12,12),
            // ClientLocation = new Location()
            // {
            //     Latitude = -23.213,
            //     Longitude = 1.123,
            // },
            ClientLocation = "-23.213, 1.123",
        };

        await _dbContext.AddAsync(trans);
        await _dbContext.SaveChangesAsync();
        
        return Ok(new RequestResult()
        {
            Success = true,
            Messages = new List<string>()
            {
                "actually works",
            },
        });
    }

    [HttpPost]
    [Route("ImportExcelData")]
    public async Task<IActionResult> ImportExcelData(IFormFile file)
    {
        // getting from file
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
        // getting from database
        var transactions = new List<TransactionsInfo>()
        {
            new ()
            {
                TransactionId = "lol1111",
                Name = "Mykola Parasuk",
                Email = "mykola322@gmail.com",
                Amount = 123.33m,
                TransactionDate = DateTime.Now,
                // ClientLocation = new ()
                // {
                //     Latitude = 54.214234,
                //     Longitude = -12.111241,
                // },
                ClientLocation = "54.214234, -12.111241",
            },
            
            new ()
            {
                TransactionId = "lol22222",
                Name = "Julia Howlk",
                Email = "howlk_julia89@gmail.com",
                Amount = 741.02m,
                TransactionDate = DateTime.Now,
                // ClientLocation = new ()
                // {
                //     Latitude = -1.111231,
                //     Longitude = 32.645233,
                // },
                ClientLocation = "-1.111231, 32.645233",
            },
        };

        var fileInBytes = _parser.WriteIntoFile(transactions);

        return File(fileInBytes, "application/octet-stream", "exported_data.csv");
    }
}