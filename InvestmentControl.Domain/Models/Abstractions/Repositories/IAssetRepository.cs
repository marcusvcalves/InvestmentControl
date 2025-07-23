using InvestmentControl.Domain.Models.Entities;
using System.Linq.Expressions;

namespace InvestmentControl.Domain.Models.Abstractions.Repositories;

public interface IAssetRepository
{
    Task<Asset?> GetAssetByCode(string code, params Expression<Func<Asset, object>>[] includes);
}
