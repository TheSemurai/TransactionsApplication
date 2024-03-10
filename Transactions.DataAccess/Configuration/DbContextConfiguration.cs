using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Transactions.DataAccess.Configuration;

public static class DbContextConfiguration
{
    public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<ApplicationContext>(
            (optionsBuilder) => optionsBuilder
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        );
        
        return services;
    }
}