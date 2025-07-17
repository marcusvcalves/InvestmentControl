using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace InvestmentControl.Infrastructure.Context;

public class BankDbContextFactory : IDesignTimeDbContextFactory<BankDbContext>
{
    private readonly IConfiguration _configuration;

    public BankDbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public BankDbContext CreateDbContext(string[] args)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<BankDbContext>();

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string is empty. Please provide a valid connection string.");
        }

        optionsBuilder.UseNpgsql(
            connectionString,
            npgsqlOptions => npgsqlOptions.UseNodaTime()
        );

        return new BankDbContext(optionsBuilder.Options);
    }
}