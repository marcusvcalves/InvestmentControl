using InvestmentControl.Domain.Models.Entities;

namespace InvestmentControl.Domain.Models.Abstractions.Repositories;

public interface IOperationRepository
{
    IQueryable<Operation> GetQueryable();
}
