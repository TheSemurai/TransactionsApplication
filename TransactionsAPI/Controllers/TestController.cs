using Microsoft.AspNetCore.Mvc;
using Transactions.DataAccess;
using Transactions.DataAccess.Entities;
using TransactionsAPI.Infrastructure;

namespace TransactionsAPI.Controllers;

public class TestController : BaseController
{
    private readonly DatabaseHandler _databaseHandler;

    public TestController(DatabaseHandler databaseHandler)
    {
        _databaseHandler = databaseHandler;
    }
    
    [HttpGet]
    [Route("TestMyFewTransInsert")]
    public async Task<IActionResult> TestMyFewTransInsert()
    {
        var transactions = new List<TransactionsInfo>()
        {
            new ()
            {
                TransactionId = Guid.NewGuid().ToString(),
                Name = "Olya Parasuk",
                Email = "olya22@gmail.com",
                Amount = 1231.33m,
                TransactionDate = DateTime.Now,
                ClientLocation = "54.214234, -12.111241",
            },
            new ()
            {
                TransactionId = Guid.NewGuid().ToString(),
                Name = "John Cena",
                Email = "cena11111@gmail.com",
                Amount = 12001.56m,
                TransactionDate = DateTime.Now,
                ClientLocation = "-1.111231, 32.645233",
            },
            new ()
            {
                TransactionId = Guid.NewGuid().ToString(),
                Name = "Raphtalia Racoon",
                Email = "raphtalia69@gmail.com",
                Amount = 333.02m,
                TransactionDate = DateTime.Now,
                ClientLocation = "-1.354431, 54.645233",
            },
            new ()
            {
                TransactionId = Guid.NewGuid().ToString(),
                Name = "Wise Holo",
                Email = "wiseHolo@gmail.com",
                Amount = 1981.09m,
                TransactionDate = DateTime.Now,
                ClientLocation = "21.333231, -51.335299",
            },
        };

        var request = await _databaseHandler.InsertTransactionsAsync(transactions);

        if (request.Success)
            return Ok(request.Messages);

        return BadRequest(request.Messages);
    }

    [HttpGet]
    [Route("TestMyInsert")]
    public async Task<IActionResult> TestMyInsert()
    {
        var trans = new TransactionsInfo()
        {
            TransactionId = Guid.NewGuid().ToString(),
            Name = "Julia Howlk",
            Email = "howlk_julia89@gmail.com",
            Amount = 741.02m,
            TransactionDate = DateTime.Now,
            // ClientLocation = new()
            // {
            //     Latitude = -1.111231,
            //     Longitude = 32.645233,
            // },
            ClientLocation = "-1.111231, 32.645233",
        };

        var request = await _databaseHandler.InsertTransactionAsync(trans);

        if (request.Success)
            return Ok(request.Messages);

        return BadRequest(request.Messages);
    }

    [HttpPost]
    [Route("OnPostUploadAsync")]
    public async Task<IActionResult> OnPostUploadAsync(IFormFile file)
    {
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Donwload()
    {
        var path = @"C:\Users\maksf\Desktop\data.csv";
        Stream stream = new FileStream(path, FileMode.Open);

        if(stream == null)
            return NotFound(); // returns a NotFoundResult with Status404NotFound response.

        return File(stream, "application/octet-stream", "exported_data_from_database.csv");
    }
    
    [HttpGet]
    [Route("testDWN")]
    public async Task<IActionResult> DonwloadTest()
    {
        var path = @"C:\Users\maksf\Desktop\data.csv";
        // Read CSV file
        var csvData = System.IO.File.ReadAllLines(path);

        // Get CSV headers
        var headers = csvData.First().Split(',');

        // Define custom mapping between CSV headers and object properties
        var propertyMappings = new Dictionary<string, string>
        {
            { "transaction_id", nameof(TransactionsInfo.TransactionId) },
            { "name", nameof(TransactionsInfo.Name) },
            { "email", nameof(TransactionsInfo.Email) },
            { "amount", nameof(TransactionsInfo.Amount) },
            { "transaction_date", nameof(TransactionsInfo.TransactionDate) },
            { "client_location", nameof(TransactionsInfo.ClientLocation) },
        };

        // Parse CSV data
        var records = new List<TransactionsInfo>();
        foreach (var line in csvData.Skip(1)) // Skip header line
        {
            var values = line.Split(',');
            var record = new TransactionsInfo();
            for (int i = 0; i < headers.Length; i++)
            {
                if (propertyMappings.TryGetValue(headers[i], out string propertyName))
                {
                    var property = typeof(TransactionsInfo).GetProperty(propertyName);
                    if (property != null && property.CanWrite)
                    {
                        // Convert value to the appropriate type and set property value
                        var convertedValue = Convert.ChangeType(values[i], property.PropertyType);
                        property.SetValue(record, convertedValue);
                    }
                }
            }
            records.Add(record);
        }

        // Use the parsed data as needed
        foreach (var record in records)
        {
            Console.WriteLine($"ID: {record.TransactionId}, Name: {record.Name}, Age: {record.Email}");
        }

        return Ok();
    }
}