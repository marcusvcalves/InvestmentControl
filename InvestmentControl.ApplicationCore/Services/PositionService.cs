using InvestmentControl.ApplicationCore.DTOs.Responses;
using InvestmentControl.Domain.Models.Entities;
using InvestmentControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InvestmentControl.ApplicationCore.Services;

public class PositionService
{
    private readonly BankDbContext _bankDbContext;
    public PositionService(BankDbContext bankDbContext)
    {
        _bankDbContext = bankDbContext ?? throw new ArgumentNullException(nameof(bankDbContext));
    }

    public async Task<ICollection<Position>> GetClientPositionsAsync(Guid userId)
    {
        var user = await _bankDbContext.Users
            .Include(u => u.Positions)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new ArgumentException($"User with ID {userId} not found.");

        return user.Positions;
    }

    public async Task<List<User>> GetTopClientsWithHighestPositionsValueAsync(int topCount = 10)
    {
        var clientsWithPositions = await _bankDbContext.Users
            .Include(u => u.Positions)
                .ThenInclude(p => p.Asset)
                    .ThenInclude(a => a.Quotations)
            .ToListAsync();

        var clientsByValue = clientsWithPositions
            .Select(user => new
            {
                User = user,
                TotalPositionValue = user.Positions.Sum(position =>
                {
                    var latestQuotation = position.Asset.Quotations
                        .OrderByDescending(q => q.CreatedAt)
                        .FirstOrDefault();

                    return latestQuotation != null ? position.Quantity * latestQuotation.UnitPrice : 0m;
                })
            })
            .Where(x => x.TotalPositionValue > 0)
            .OrderByDescending(x => x.TotalPositionValue)
            .Take(topCount)
            .ToList();

        return clientsByValue.Select(x => x.User).ToList();
    }

    public async Task<List<AssetAveragePrice>> GetAveragePricePerAssetForUserAsync(Guid userId)
    {
        var userPositions = await _bankDbContext.Positions
            .Include(p => p.Asset)
            .Where(p => p.UserId == userId)
            .Select(p => new
            {
                AssetId = p.AssetId,
                AssetCode = p.Asset.Code,
                AssetName = p.Asset.Name,
                Quantity = p.Quantity,
                MediumPrice = p.MediumPrice
            })
            .ToListAsync();

        if (userPositions.Count == 0)
        {
            return new List<AssetAveragePrice>();
        }

        var result = userPositions.Select(p => new AssetAveragePrice
        {
            AssetId = p.AssetId,
            AssetCode = p.AssetCode,
            AssetName = p.AssetName,
            CurrentQuantity = p.Quantity,
            AveragePrice = p.MediumPrice
        }).ToList();

        return result;
    }
}
