namespace Transactions.DataAccess;

public class RequestResult
{
    public bool Success { get; set; }
    public List<string>? Messages { get; set; }
}