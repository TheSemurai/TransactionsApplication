namespace TransactionsAPI.Infrastructure.Interfaces;

public interface IReader<T>
{
    List<T> ReadFromFile(IFormFile file);
}