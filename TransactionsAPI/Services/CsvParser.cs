using System.Globalization;
using System.Reflection;
using CsvHelper;
using TransactionsAPI.Entities;
using TransactionsAPI.Infrastructure.CsvHelperConfiguration;
using TransactionsAPI.Infrastructure.Interfaces;

namespace TransactionsAPI.Services;

/// <summary>
/// Parser for TransactionsInfoModel
/// </summary>
public class CsvParser : IParser<TransactionsInfoModel>
{
    /// <summary>
    /// Read records from file
    /// </summary>
    /// <param name="file">Specific file data</param>
    /// <returns>Collection of TransactionInfo model</returns>
    public List<TransactionsInfoModel> ReadFromFile(IFormFile file)
    {
        using (var reader = new StreamReader(file.OpenReadStream()))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<TransactionsInfoMap>();
            var records = csv.GetRecords<TransactionsInfoModel>();

            return records.ToList();
        }
    }

    /// <summary>
    /// Write into file with static header
    /// </summary>
    /// <param name="list">Collection of transactions</param>
    /// <returns>array of bytes</returns>
    public byte[] WriteIntoFile(List<TransactionsInfoModel> list)
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
    
    /// <summary>
    /// Writing into file with custom header
    /// </summary>
    /// <param name="list">Collection of transactions</param>
    /// <param name="exportedColumns">Selected properties to export</param>
    /// <returns>array of bytes</returns>
    public byte[] WriteIntoFileWithCustomHeader(List<TransactionsInfoModel> list, ExportedColumns exportedColumns)
    {
        var dict = new Dictionary<string, string>()
        {
            { "TransactionId", "transaction_id" },
            { "Name", "name" },
            { "Email", "email" },
            { "Amount", "amount" },
            { "TransactionDate", "transaction_date" },
            { "ClientLocation", "client_location" },
        };
        
        using (var memoryStream = new MemoryStream())
        using (var streamWriter = new StreamWriter(memoryStream))
        using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
        {
            csvWriter.Context.RegisterClassMap<TransactionsInfoMap>();
            
            Type exportedColumnsType = typeof(ExportedColumns);
            PropertyInfo[] properties = exportedColumnsType.GetProperties();

            // Write header data by exported columns
            foreach (PropertyInfo property in properties)
            {
                if ((bool)property.GetValue(exportedColumns)!)
                {
                    csvWriter.WriteField(dict[property.Name]);
                }
            }
            csvWriter.NextRecord();

            // Write records based on exported columns
            foreach (var item in list)
            {
                foreach (PropertyInfo property in properties)
                {
                    if ((bool)property.GetValue(exportedColumns)!)
                    {
                        object value;

                        if (property.Name == "TransactionDate")
                            value = TimeZoneInfo.ConvertTimeToUtc(item.TransactionDate.DateTime, 
                                TimeZoneService.ConvertToTimeZoneInfo(item.ClientLocation)).ToString("yyyy-MM-dd HH:mm:ss");
                        else 
                            value = typeof(TransactionsInfoModel).GetProperty(property.Name).GetValue(item);
                        
                        csvWriter.WriteField(value);
                    }
                }
                csvWriter.NextRecord();
            }

            csvWriter.Flush();
            return memoryStream.ToArray();
        }
    }
}