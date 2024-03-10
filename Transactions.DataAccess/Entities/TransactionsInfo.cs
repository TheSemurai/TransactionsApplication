using System.ComponentModel.DataAnnotations;

namespace Transactions.DataAccess.Entities;

public class TransactionsInfo
{
    [Key]
    public string TransactionId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    
    public Location ClientLocation { get; set; }
}

