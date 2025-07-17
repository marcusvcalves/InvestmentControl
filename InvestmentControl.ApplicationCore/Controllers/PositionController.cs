using InvestmentControl.ApplicationCore.DTOs.Responses;
using InvestmentControl.ApplicationCore.Extensions;
using InvestmentControl.ApplicationCore.Services;
using InvestmentControl.Domain.Models;
using InvestmentControl.Domain.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentControl.ApplicationCore.Controllers;

[ApiController]
[Route("positions")]
public class PositionController : ControllerBase
{
    private readonly PositionService _positionService;

    public PositionController(PositionService positionService)
    {
        _positionService = positionService;
    }

    /// <summary>
    /// Retorna os 10 clientes com maiores posições.
    /// </summary>
    /// <returns>Clientes com maiores posições.</returns>
    [HttpGet("highest")]
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopClientsWithHighestPositionsValueAsync()
    {
        var users = await _positionService.GetTopClientsWithHighestPositionsValueAsync();

        return Ok(users);
    }

    /// <summary>
    /// Retorna os 10 clientes que pagaram mais corretagem.
    /// </summary>
    /// <returns>Clientes que pagaram mais corretagem.</returns>
    [HttpGet("brokerage/highest")]
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopClientsWhoPaidMostBrokerageAsync()
    {
        var users = await _positionService.GetTopClientsWhoPaidMostBrokerageAsync();

        return Ok(users);
    }

    /// <summary>
    /// Retorna as posições de um cliente específico.
    /// </summary>
    /// <param name="userId">ID do cliente para trazer as posições.</param>
    /// <returns>Posições do cliente.</returns>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(ICollection<Position>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClientPositionsAsync([FromRoute] Guid userId)
    {
        var result = await _positionService.GetClientPositionsAsync(userId);

        return result.Match<ICollection<Position>, IActionResult>(
            onSuccess: Ok,
            onFailure: NotFound
        );
    }

    /// <summary>
    /// Retorna as a posição de um cliente em determinado ativo específico.
    /// </summary>
    /// <param name="userId">ID do cliente para trazer as posições.</param>
    /// <param name="assetCode">Código do ativo para buscar a posição.</param>
    /// <returns>Posição do cliente.</returns>
    [HttpGet("{userId:guid}/{assetCode}/last")]
    [ProducesResponseType(typeof(Position), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClientPositionInAssetAsync([FromRoute] Guid userId, [FromRoute] string assetCode)
    {
        var result = await _positionService.GetClientPositionInAssetAsync(userId, assetCode);

        return result.Match<Position, IActionResult>(
            onSuccess: Ok,
            onFailure: NotFound
        );
    }

    /// <summary>
    /// Retorna o preço médio ponderado por ativo de um cliente específico.
    /// </summary>
    /// <param name="userId">ID do cliente para trazer o preço médio ponderado.</param>
    /// <returns>Preço médio ponderado por ativo para o cliente.</returns>
    [HttpGet("{userId:guid}/average-price-per-asset")]
    [ProducesResponseType(typeof(List<AssetAveragePrice>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAveragePricePerAssetForUserAsync(Guid userId)
    {
        var result = await _positionService.GetAveragePricePerAssetForUserAsync(userId);

        return result.Match<List<AssetAveragePrice>, IActionResult>(
            onSuccess: Ok,
            onFailure: NotFound
        );
    }
}
