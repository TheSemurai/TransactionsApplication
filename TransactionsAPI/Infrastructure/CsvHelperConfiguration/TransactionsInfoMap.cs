using CsvHelper.Configuration;
using TransactionsAPI.Entities;
using TransactionsAPI.Infrastructure.CsvHelperConfiguration.Convertors;

namespace TransactionsAPI.Infrastructure.CsvHelperConfiguration;

/// <summary>
/// Class for mapping a TransactionsInfoModel
/// </summary>
public class TransactionsInfoMap : ClassMap<TransactionsInfoModel>
{
    public TransactionsInfoMap()
    {
        Map(x => x.TransactionId)
            .Index(0)
            .Name("transaction_id");
        
        Map(x => x.Name)
            .Index(1)
            .Name("name");
        
        Map(x => x.Email)
            .Index(2)
            .Name("email");
        
        Map(x => x.Amount)
            .Index(3)
            .Name("amount");
            //.TypeConverter<AmountConvertor>();
        
        Map(x => x.TransactionDate)
            .Index(4)
            .Name("transaction_date")
            .TypeConverter<DateTimeFormatConvertor>();

        Map(x => x.ClientLocation)
            .Index(5)
            .Name("client_location");
        //.TypeConverter<LocationConverter>();
    }
}