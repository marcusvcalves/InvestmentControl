using InvestmentControl.Domain.Models.Abstractions.Repositories;
using InvestmentControl.Domain.Models.Entities;
using InvestmentControl.Infrastructure.Context;

namespace InvestmentControl.Infrastructure.Repositories;
public class OperationRepository : IOperationRepository
{
    private readonly BankDbContext _bankDbContext;
    public OperationRepository(BankDbContext bankDbContext)
    {
        _bankDbContext = bankDbContext ?? throw new ArgumentNullException(nameof(bankDbContext));
    }

    public IQueryable<Operation> GetQueryable()
    {
        return _bankDbContext.Operations.AsQueryable();
    }
}
