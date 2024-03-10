using Microsoft.EntityFrameworkCore;
using Transactions.DataAccess.Configuration;
using Transactions.DataAccess.Entities;

namespace Transactions.DataAccess;

public class ApplicationContext : DbContext
{
    public DbSet<TransactionsInfo> Transactions { get; set; }
    
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) {}
    
    protected void OnConfiguring(DbContextOptionsBuilder<ApplicationContext> optionsBuilder) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");

        modelBuilder.ApplyConfiguration(new TransactionsConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}