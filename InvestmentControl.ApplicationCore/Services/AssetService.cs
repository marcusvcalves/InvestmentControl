using InvestmentControl.ApplicationCore.DTOs.Responses;
using InvestmentControl.Domain.Models;
using InvestmentControl.Domain.Models.Abstractions.Repositories;
using System.Net;

namespace InvestmentControl.ApplicationCore.Services;

public class AssetService
{
    private readonly IAssetRepository _assetRepository;
    public AssetService(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository ?? throw new ArgumentNullException(nameof(assetRepository));
    }

    public async Task<Result> GetAssetLastQuotationByCodeAsync(string code)
    {
        var asset = await _assetRepository.GetAssetByCodeAsync(code, includes: x => x.Quotations);

        if (asset is null)
        {
            return Result.Failure(new ApiErrorResponse(HttpStatusCode.NotFound, $"Asset with code {code} not found."));
        }

        var lastQuotation = asset.Quotations
            .OrderByDescending(q => q.CreatedAt)
            .FirstOrDefault();

        if (lastQuotation is null)
        {
            return Result.Failure(new ApiErrorResponse(HttpStatusCode.NotFound, $"No quotations found for asset with code {code}."));
        }

        var quotationResponse = new QuotationResponse
        {
            Id = lastQuotation.Id,
            AssetId = lastQuotation.AssetId,
            UnitPrice = lastQuotation.UnitPrice,
            TimestampUtc = lastQuotation.CreatedAt
        };


        return Result.Success(quotationResponse);
    }
}
