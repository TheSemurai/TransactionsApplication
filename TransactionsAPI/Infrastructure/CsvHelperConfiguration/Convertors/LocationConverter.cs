using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Transactions.DataAccess.Entities;

namespace TransactionsAPI.Infrastructure.CsvHelperConfiguration.Convertors;

// public class LocationConverter : DefaultTypeConverter
// {
//     public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
//     {
//         var content = text.Split(',');
//         
//         return new Location()
//         {
//             Latitude = double.Parse(content[0]), 
//             Longitude = double.Parse(content[1]),
//         };
//     }
//
//     public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
//     {
//         var location = (Location) value;
//         return $"{location.Latitude}, {location.Longitude}";
//     }
// }