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
    public async Task<IResult> GetLatestQuotationByAssetCodeAsync([FromRoute] string assetCode)
    {
        var result = await _assetService.GetAssetLastQuotationByCodeAsync(assetCode);


        return result.Match<QuotationResponse, IResult>(
            onSuccess: quotation => Results.Ok(quotation),
            onFailure: error => Results.NotFound(new { error.HttpStatusCode, error.Description })
        );
    }
}
