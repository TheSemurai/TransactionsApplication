using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace TransactionsAPI.Infrastructure.CsvHelperConfiguration.Convertors;

/// <summary>
/// Convertor of date time offset data for csv parser
/// </summary>
public class DateTimeFormatConvertor : DefaultTypeConverter
{
    private const string pattern = "yyyy-MM-dd HH:mm:ss";

    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        => DateTime.Parse(text);
    // return DateTimeOffset.ParseExact(text, pattern, CultureInfo.InvariantCulture);

    public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        => ((DateTime)value).ToString(pattern, CultureInfo.InvariantCulture);
    // return ((DateTimeOffset) value).ToString(pattern, CultureInfo.InvariantCulture);
}