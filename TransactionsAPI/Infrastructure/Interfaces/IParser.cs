using Microsoft.AspNetCore.Mvc;

namespace TransactionsAPI.Infrastructure.Interfaces;

public interface IParser<T>
{
    List<T> ReadFromFile(IFormFile file);
    byte[] WriteIntoFile(List<T> list);
}