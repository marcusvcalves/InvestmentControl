using InvestmentControl.Domain.Models.Abstractions.Repositories;
using InvestmentControl.Domain.Models.Entities;
using InvestmentControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InvestmentControl.Infrastructure.Repositories;
public class AssetRepository : IAssetRepository
{
    private readonly BankDbContext _bankDbContext;
    public AssetRepository(BankDbContext bankDbContext)
    {
        _bankDbContext = bankDbContext ?? throw new ArgumentNullException(nameof(bankDbContext));
    }

    public async Task<Asset?> GetAssetByCodeAsync(string code, params Expression<Func<Asset, object>>[] includes)
    {
        IQueryable<Asset> query = _bankDbContext.Assets;

        foreach (var includeProperty in includes)
        {
            query = query.Include(includeProperty);
        }

        return await query
            .Where(a => a.Code == code)
            .FirstOrDefaultAsync();
    }
}
