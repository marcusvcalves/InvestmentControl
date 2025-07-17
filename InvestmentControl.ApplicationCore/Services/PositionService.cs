using InvestmentControl.ApplicationCore.DTOs.Responses;
using InvestmentControl.Domain.Models;
using InvestmentControl.Domain.Models.Entities;
using InvestmentControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InvestmentControl.ApplicationCore.Services;

public class PositionService
{
    private readonly BankDbContext _bankDbContext;
    public PositionService(BankDbContext bankDbContext)
    {
        _bankDbContext = bankDbContext ?? throw new ArgumentNullException(nameof(bankDbContext));
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

    public async Task<List<User>> GetTopClientsWhoPaidMostBrokerageAsync(int topCount = 10)
    {
        var topClientsWithBrokerage = await _bankDbContext.Operations
            .GroupBy(op => op.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalBrokerage = g.Sum(op => op.Brokerage)
            })
            .OrderByDescending(x => x.TotalBrokerage)
            .Take(topCount)
            .ToListAsync();

        var topClientIds = topClientsWithBrokerage.Select(x => x.UserId).ToList();

        var topClients = await _bankDbContext.Users
            .Where(u => topClientIds.Contains(u.Id))
            .ToListAsync();

        var orderedTopClients = topClients
            .Join(topClientsWithBrokerage,
                  user => user.Id,
                  brokerageResult => brokerageResult.UserId,
                  (user, brokerageResult) => new { User = user, brokerageResult.TotalBrokerage })
            .OrderByDescending(x => x.TotalBrokerage)
            .Select(x => x.User)
            .ToList();

        return orderedTopClients;
    }

    public async Task<Result> GetClientPositionsAsync(Guid userId)
    {
        var user = await _bankDbContext.Users
            .Include(u => u.Positions)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return Result.Failure(new ApiErrorResponse(HttpStatusCode.NotFound, $"User with ID {userId} not found."));
        }

        return Result.Success(user.Positions);
    }

    public async Task<Result> GetClientPositionInAssetAsync(Guid userId, string assetCode)
    {
        var user = await _bankDbContext.Users
            .Include(u => u.Positions)
                .ThenInclude(p => p.Asset)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return Result.Failure(new ApiErrorResponse(HttpStatusCode.NotFound, $"User with ID {userId} not found."));
        }

        var position = user.Positions.FirstOrDefault(p => p.Asset.Code == assetCode);

        if (position is null)
        {
            return Result.Failure(new ApiErrorResponse(HttpStatusCode.NotFound, $"No position found for asset code {assetCode} for user with ID {userId}."));
        }

        return Result.Success(position);
    }

    public async Task<Result> GetAveragePricePerAssetForUserAsync(Guid userId)
    {
        var userPositionsRaw = await _bankDbContext.Positions
            .Include(p => p.Asset)
            .Where(p => p.UserId == userId)
            .Select(p => new
            {
                p.AssetId,
                p.Asset.Code,
                p.Asset.Name,
                p.Quantity,
                p.MediumPrice
            })
            .ToListAsync();

        if (userPositionsRaw is null || userPositionsRaw.Count == 0)
        {
            return Result.Failure(new ApiErrorResponse(HttpStatusCode.NotFound, $"No positions found for user with ID {userId}."));
        }

        var assetAveragePrices = userPositionsRaw
            .GroupBy(p => new { p.AssetId, p.Code, p.Name })
            .Select(g =>
            {
                var totalQuantity = g.Sum(p => p.Quantity);
                var totalWeightedCost = g.Sum(p => (decimal)p.Quantity * p.MediumPrice);

                return new AssetAveragePrice
                {
                    AssetId = g.Key.AssetId,
                    AssetCode = g.Key.Code,
                    AssetName = g.Key.Name,
                    CurrentQuantity = totalQuantity,
                    AveragePrice = totalQuantity > 0 ? totalWeightedCost / totalQuantity : 0m
                };
            })
            .ToList();

        if (assetAveragePrices.Count == 0)
        {
            return Result.Failure(new ApiErrorResponse(HttpStatusCode.NotFound, $"No active positions found for user with ID {userId}."));
        }

        return Result.Success(assetAveragePrices);
    }
}
