using System.Text;
using Microsoft.AspNetCore.Mvc;
using Transactions.DataAccess.Entities;
using TransactionsAPI.Infrastructure.Interfaces;

namespace TransactionsAPI.Services;

public class CsvParser : IParser<TransactionsInfo>
{
    public List<TransactionsInfo> ReadFromFile(IFormFile file)
    {
        return new();
    }

    public MemoryStream WriteIntoFile(List<TransactionsInfo> list)
    {
        var path = "export_data.csv";
        
        MemoryStream memoryStream = new MemoryStream();
        
        var header = "transaction_id,name,email,amount,transaction_date,client_location\n";
        byte[] headerInBytes = Encoding.UTF8.GetBytes(header);
        memoryStream.Write(headerInBytes, 0, headerInBytes.Length);

        foreach (var item in list)
        {
            var text = $"{item.TransactionId},{item.Name},{item.Email},{item.Amount},{item.TransactionDate},\"{item.ClientLocation.Latitude}, {item.ClientLocation.Longitude}\"\n";
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            memoryStream.Write(textBytes, 0, textBytes.Length);
        }
        
        memoryStream.Seek(0, SeekOrigin.Begin);
        
        return memoryStream;
    }
}

