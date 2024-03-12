using Microsoft.AspNetCore.Mvc;

namespace TransactionsAPI.Infrastructure.Interfaces;

public interface IParser<T>
{
    List<T> ReadFromFile(IFormFile file);
    MemoryStream WriteIntoFile(List<T> list);
}