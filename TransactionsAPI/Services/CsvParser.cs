using System.Globalization;
using System.Reflection;
using CsvHelper;
using TransactionsAPI.Entities;
using TransactionsAPI.Infrastructure.CsvHelperConfiguration;
using TransactionsAPI.Infrastructure.Interfaces;

namespace TransactionsAPI.Services;

public class CsvParser : IParser<TransactionsInfoModel>
{
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
    
    public byte[] WriteIntoFileWithCustomHeader(List<TransactionsInfoModel> list, ExportedColumns exportedColumns)
    {
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
                if ((bool)property.GetValue(exportedColumns))
                {
                    csvWriter.WriteField(property.Name);
                }
            }
            csvWriter.NextRecord();

            // Write records based on exported columns
            foreach (var item in list)
            {
                foreach (PropertyInfo property in properties)
                {
                    if ((bool)property.GetValue(exportedColumns))
                    {
                        object value = typeof(TransactionsInfoModel).GetProperty(property.Name).GetValue(item);
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