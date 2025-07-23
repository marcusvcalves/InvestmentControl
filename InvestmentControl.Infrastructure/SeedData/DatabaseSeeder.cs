using InvestmentControl.Domain.Models.Abstractions.Repositories;
using InvestmentControl.Domain.Models.Entities;
using InvestmentControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace InvestmentControl.Infrastructure.SeedData;
public class DatabaseSeeder
{
    private readonly BankDbContext _bankDbContext;
    private readonly IUserRepository _userRepository;
    public DatabaseSeeder(BankDbContext bankDbContext, IUserRepository userRepository)
    {
        _bankDbContext = bankDbContext ?? throw new ArgumentNullException(nameof(bankDbContext));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task SeedAsync()
    {
        await _bankDbContext.Database.OpenConnectionAsync();
        using (var command = _bankDbContext.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "CREATE EXTENSION IF NOT EXISTS citext";
            await command.ExecuteNonQueryAsync();
        }

        ((NpgsqlConnection)_bankDbContext.Database.GetDbConnection()).ReloadTypes();


        await SeedUsersData();
    }

    private async Task SeedUsersData()
    {
        Guid user1ID = Guid.Parse("b97e1f05-14ea-4683-b7df-ea1907660aac");
        Guid user2ID = Guid.Parse("c1f3b8d0-4e5a-4c9b-8f6d-7e8f9a0b1c2d");
        Guid user3ID = Guid.Parse("3c1b1926-6760-4a35-908a-c0a50c159e99");

        if (await _userRepository.GetByIdAsync(user1ID) is null)
        {
            var user1 = User.Create(
                user1ID,
                "João da Silva",
                "joaodasilva@email.com",
                0.1m
            );

            await _userRepository.AddAsync(user1, saveChangesAsync: false);
        }

        if (await _userRepository.GetByIdAsync(user2ID) is null)
        {
            var user2 = User.Create(
                user2ID,
                "Maria Oliveira",
                "mariaoliveira@email.com",
                0.2m
            );

            await _userRepository.AddAsync(user2, saveChangesAsync: false);
        }

        if (await _userRepository.GetByIdAsync(user3ID) is null)
        {
            var user3 = User.Create(
                user3ID,
                "José",
                "jose@email.com",
                0.15m
            );

            await _userRepository.AddAsync(user3, saveChangesAsync: false);
        }

        await _bankDbContext.SaveChangesAsync();
    }
}
