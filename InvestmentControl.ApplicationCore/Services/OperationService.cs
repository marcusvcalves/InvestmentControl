using InvestmentControl.Domain.Models.Entities;
using InvestmentControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InvestmentControl.ApplicationCore.Services;

public class OperationService
{
    private readonly BankDbContext _bankDbContext;
    public OperationService(BankDbContext bankDbContext)
    {
        _bankDbContext = bankDbContext ?? throw new ArgumentNullException(nameof(bankDbContext));
    }

    public async Task<decimal> GetTotalBrokerageGainAsync()
    {
        var totalBrokerage = await _bankDbContext.Operations
                                        .SumAsync(op => op.Brokerage);

        return totalBrokerage;
    }

    public async Task<List<User>> GetTopClientsByBrokeragePaidAsync(int topCount = 10)
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
}
