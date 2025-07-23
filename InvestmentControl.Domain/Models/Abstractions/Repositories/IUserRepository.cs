using InvestmentControl.Domain.Models.Entities;

namespace InvestmentControl.Domain.Models.Abstractions.Repositories;

public interface IUserRepository
{
    IQueryable<User> GetQueryable();
}
