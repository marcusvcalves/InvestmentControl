using InvestmentControl.ApplicationCore.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace InvestmentControl.Infrastructure.Context;

public class BankDbContextFactory : IDesignTimeDbContextFactory<BankDbContext>
{
    public BankDbContextFactory()
    {

    }

    public BankDbContext CreateDbContext(string[] args)
    {
        string? solutionPath = PathUtils.GetSolutionPath();

        if (string.IsNullOrEmpty(solutionPath))
        {
            throw new InvalidOperationException("Solution path could not be determined. Ensure you are running this from a valid project directory.");
        }

        string appSettingsPath = solutionPath + @"\InvestmentControl.ApplicationCore\appsettings.Development.json";

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile(appSettingsPath)
            .Build();


        var builder = new DbContextOptionsBuilder<BankDbContext>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

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