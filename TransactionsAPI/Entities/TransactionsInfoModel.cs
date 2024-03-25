using System.ComponentModel.DataAnnotations;
using MiniExcelLibs.Attributes;

namespace TransactionsAPI.Entities;

public class TransactionsInfoModel
{
    [Key]
    [ExcelColumn(Name = "transaction_id", Width = 30)]
    public string TransactionId { get; set; }
    [ExcelColumn(Name = "name", Width = 25)]
    public string Name { get; set; }
    [ExcelColumn(Name = "email", Width = 25)]
    public string Email { get; set; }
    [ExcelColumn(Name = "amount", Width = 15)]
    public string Amount { get; set; }
    [ExcelColumn(Name = "transaction_date", Format = "yyyy-MM-dd HH:mm:ss", Width = 25)]
    public DateTime TransactionDate { get; set; }
    [ExcelColumn(Name = "client_location", Width = 30)]
    public string ClientLocation { get; set; }
}