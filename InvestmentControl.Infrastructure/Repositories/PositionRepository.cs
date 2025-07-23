using InvestmentControl.Domain.Models.Abstractions.Repositories;
using InvestmentControl.Domain.Models.Entities;
using InvestmentControl.Infrastructure.Context;

namespace InvestmentControl.Infrastructure.Repositories;
public class PositionRepository : IPositionRepository
{
    private readonly BankDbContext _bankDbContext;
    public PositionRepository(BankDbContext bankDbContext)
    {
        _bankDbContext = bankDbContext ?? throw new ArgumentNullException(nameof(bankDbContext));
    }

    public IQueryable<Position> GetQueryable()
    {
        return _bankDbContext.Positions.AsQueryable();
    }
}
