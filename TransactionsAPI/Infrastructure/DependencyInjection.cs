using Transactions.DataAccess;
using Transactions.DataAccess.Entities;
using TransactionsAPI.Entities;
using TransactionsAPI.Infrastructure.Interfaces;
using TransactionsAPI.Services;

namespace TransactionsAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureServiceCollection(this IServiceCollection services)
    {
        services.AddTransient<IParser<TransactionsInfoModel>, CsvParser>();
        services.AddScoped<DatabaseHandler>();

        return services;
    }
}