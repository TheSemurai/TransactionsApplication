namespace TransactionsAPI.Infrastructure.Interfaces;

public interface IParser<T> : IReader<T>, IWriter<T>
{
}