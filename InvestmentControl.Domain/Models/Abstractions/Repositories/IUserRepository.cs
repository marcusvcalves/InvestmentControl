using InvestmentControl.Domain.Models.Entities;

namespace InvestmentControl.Domain.Models.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    IQueryable<User> GetQueryable();
    Task<User> AddAsync(User user, bool saveChangesAsync);
}
