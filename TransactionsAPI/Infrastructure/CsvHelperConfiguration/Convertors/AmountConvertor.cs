using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace TransactionsAPI.Infrastructure.CsvHelperConfiguration.Convertors;

public class AmountConvertor : DefaultTypeConverter 
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        return decimal.Parse(text.Substring(1));
    }

    public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
    {
        return $"${value}";
    }
}