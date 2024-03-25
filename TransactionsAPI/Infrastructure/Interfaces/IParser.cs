using TransactionsAPI.Entities;

namespace TransactionsAPI.Infrastructure.Interfaces;

public interface IParser<T>
{
    List<T> ReadFromFile(IFormFile file);
    byte[] WriteIntoFile(List<T> list);
    byte[] WriteIntoFileWithCustomHeader(IList<TransactionsInfoModel> list, ExportedColumns exportedColumns);
}