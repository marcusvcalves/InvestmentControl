using InvestmentControl.Domain.Models.Abstractions.Repositories;
using InvestmentControl.Domain.Models.Entities;
using InvestmentControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InvestmentControl.Infrastructure.Repositories;
public class UserRepository : IUserRepository
{
    private readonly BankDbContext _bankDbContext;
    public UserRepository(BankDbContext bankDbContext)
    {
        _bankDbContext = bankDbContext;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _bankDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public IQueryable<User> GetQueryable()
    {
        return _bankDbContext.Users.AsQueryable();
    }

    public async Task AddAsync(User user, bool saveChangesAsync)
    {
        ArgumentNullException.ThrowIfNull(user);

        await _bankDbContext.Users.AddAsync(user);

        if (saveChangesAsync)
        {
            await _bankDbContext.SaveChangesAsync();
        }
    }
}
