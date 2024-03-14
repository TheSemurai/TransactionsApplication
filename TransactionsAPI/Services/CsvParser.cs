using System.Globalization;
using CsvHelper;
using Transactions.DataAccess.Entities;
using TransactionsAPI.Infrastructure.CsvHelperConfiguration;
using TransactionsAPI.Infrastructure.Interfaces;

namespace TransactionsAPI.Services;

public class CsvParser : IParser<TransactionsInfo>
{
    public List<TransactionsInfo> ReadFromFile(IFormFile file)
    {
        using (var reader = new StreamReader(file.OpenReadStream()))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<TransactionsInfoMap>();
            var records = csv.GetRecords<TransactionsInfo>();

            return records.ToList();
        }
    }

    public byte[] WriteIntoFile(List<TransactionsInfo> list)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.Context.RegisterClassMap<TransactionsInfoMap>();
                csvWriter.WriteRecords(list);
            }

            return memoryStream.ToArray();
        }
    }
}