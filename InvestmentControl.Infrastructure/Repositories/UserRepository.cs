using InvestmentControl.Domain.Models.Abstractions.Repositories;
using InvestmentControl.Domain.Models.Entities;
using InvestmentControl.Infrastructure.Context;

namespace InvestmentControl.Infrastructure.Repositories;
public class UserRepository : IUserRepository
{
    private readonly BankDbContext _bankDbContext;
    public UserRepository(BankDbContext bankDbContext)
    {
        _bankDbContext = bankDbContext;
    }

    public IQueryable<User> GetQueryable()
    {
        return _bankDbContext.Users.AsQueryable();
    }
}
