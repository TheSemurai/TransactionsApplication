using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transactions.DataAccess;
using Transactions.DataAccess.Entities;
using TransactionsAPI.Infrastructure;

namespace TransactionsAPI.Controllers;

public class TransactionsController : BaseController
{
    private readonly ApplicationContext _dbContext;

    public TransactionsController(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
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
            ClientLocation = new Location()
            {
                Latitude = -23.213,
                Longitude = 1.123,
            },
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
}