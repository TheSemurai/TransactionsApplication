using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace TransactionsAPI.Infrastructure.CsvHelperConfiguration.Convertors;

public class DateTimeFormatConvertor : DefaultTypeConverter
{
    private const string pattern = "yyyy-MM-dd HH:mm:ss";
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        return DateTimeOffset.ParseExact(text, pattern, CultureInfo.InvariantCulture);
    }

    public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
    {
        return ((DateTimeOffset) value).ToString(pattern, CultureInfo.InvariantCulture);
    }
}