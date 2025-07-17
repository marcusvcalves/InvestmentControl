using InvestmentControl.ApplicationCore.DTOs.Responses;
using InvestmentControl.ApplicationCore.Extensions;
using InvestmentControl.ApplicationCore.Services;
using InvestmentControl.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentControl.ApplicationCore.Controllers;

[ApiController]
[Route("assets")]
public class AssetController : ControllerBase
{
    private readonly AssetService _assetService;
    public AssetController(AssetService assetService)
    {
        _assetService = assetService;
    }

    /// <summary>
    /// Retorna a última cotação para um ativo específico.
    /// </summary>
    /// <param name="assetCode">O código único do ativo (exemplo: PETR4, VALE3).</param>
    /// <returns>A última cotação do ativo.</returns>
    [HttpGet("{assetCode}/latest-quotation")]
    [ProducesResponseType(typeof(QuotationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLatestQuotationByAssetCodeAsync([FromRoute] string assetCode)
    {
        var result = await _assetService.GetAssetLastQuotationByCodeAsync(assetCode);

        return result.Match<QuotationResponse, IActionResult>(
            onSuccess: Ok,
            onFailure: NotFound
        );
    }
}
