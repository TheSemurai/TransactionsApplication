using System.ComponentModel.DataAnnotations;

namespace TransactionsAPI.Entities;

public class TransactionsInfoModel
{
    [Key]
    public string TransactionId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Amount { get; set; }
    public DateTimeOffset TransactionDate { get; set; }
    public string ClientLocation { get; set; }
}