using InvestmentControl.Domain.Models.Abstractions.Repositories;
using InvestmentControl.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestmentControl.ApplicationCore.Services;

public class OperationService
{
    private readonly IOperationRepository _operationRepository;
    private readonly IUserRepository _userRepository;
    public OperationService(IOperationRepository operationRepository, IUserRepository userRepository)
    {
        _operationRepository = operationRepository ?? throw new ArgumentNullException(nameof(operationRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<decimal> GetTotalBrokerageGainAsync()
    {
        var totalBrokerage = await _operationRepository.GetQueryable().SumAsync(op => op.Brokerage);

        return totalBrokerage;
    }

    public async Task<List<User>> GetTopClientsByBrokeragePaidAsync(int topCount = 10)
    {
        var topClientsWithBrokerage = await _operationRepository.GetQueryable()
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

        var topClients = await _userRepository.GetQueryable()
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
