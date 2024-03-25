using System.Reflection;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using MiniExcelLibs.OpenXml;
using TransactionsAPI.Entities;
using TransactionsAPI.Infrastructure.Interfaces;

namespace TransactionsAPI.Services;

public class ExcelParser : IParser<TransactionsInfoModel>
{
    /// <summary>
    /// Unfortunately, is not implemented for now
    /// </summary>
    /// <param name="file">Specific file data</param>
    /// <returns>List of transactions models</returns>
    /// <exception cref="NotImplementedException">Unfortunately, is not implemented for now</exception>
    public List<TransactionsInfoModel> ReadFromFile(IFormFile file)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Unfortunately, is not implemented for now
    /// </summary>
    /// <param name="list">Specific list of transaction models</param>
    /// <returns>Array of bytes</returns>
    /// <exception cref="NotImplementedException">Unfortunately, is not implemented for now</exception>
    public byte[] WriteIntoFile(List<TransactionsInfoModel> list)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creation some data to essential format
    /// </summary>
    /// <param name="list">Specific list of transaction models</param>
    /// <param name="exportedColumns">Some chosen fields</param>
    /// <returns>Array of bytes</returns>
    public byte[] WriteIntoFileWithCustomHeader(IList<TransactionsInfoModel> list, ExportedColumns exportedColumns)
    {
        var config = CreateCustomHeaderConfiguration(exportedColumns);

        using var memoryStream = new MemoryStream();
        memoryStream.SaveAs(list, configuration: config);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream.ToArray();
    }

    /// <summary>
    /// Configure a custom header by chosen fields 
    /// </summary>
    /// <param name="exportedColumns">Some chosen fields</param>
    /// <returns>Configuration file for excel (OpenXmlConfiguration)</returns>
    private OpenXmlConfiguration CreateCustomHeaderConfiguration(ExportedColumns exportedColumns)
    {
        Type exportedColumnsType = typeof(ExportedColumns);
        PropertyInfo[] properties = exportedColumnsType.GetProperties();
        List<string> keys = new List<string>();
        
        foreach (PropertyInfo property in properties)
        {
            if ((bool)property.GetValue(exportedColumns) is not true)
            {
                keys.Add(property.Name);
            }
        }
        
        return new OpenXmlConfiguration
        {
            DynamicColumns = keys.Select(x => 
                new DynamicExcelColumn(x)
                {
                    Ignore = true,
                }).ToArray()
        };
    }
}