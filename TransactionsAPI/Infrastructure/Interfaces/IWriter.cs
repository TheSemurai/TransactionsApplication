using TransactionsAPI.Entities;

namespace TransactionsAPI.Infrastructure.Interfaces;

public interface IWriter<T>
{
    byte[] WriteIntoFile(List<T> list);
    byte[] WriteIntoFileWithCustomHeader(IList<TransactionsInfoModel> list, ExportedColumns exportedColumns);   
}