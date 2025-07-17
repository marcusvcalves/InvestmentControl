using InvestmentControl.ApplicationCore.DTOs.Responses;
using InvestmentControl.Domain.Models;
using InvestmentControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InvestmentControl.ApplicationCore.Services;

public class AssetService
{
    private readonly BankDbContext _bankDbContext;
    public AssetService(BankDbContext bankDbContext)
    {
        _bankDbContext = bankDbContext ?? throw new ArgumentNullException(nameof(bankDbContext));
    }

    public async Task<Result> GetAssetLastQuotationByCodeAsync(string code)
    {
        var asset = await _bankDbContext.Assets
            .Where(a => a.Code == code)
            .Select(a => new
            {
                Asset = a,
                LastQuotation = a.Quotations
                                 .OrderByDescending(q => q.CreatedAt)
                                 .Select(q => new QuotationResponse
                                 {
                                     Id = q.Id,
                                     AssetId = q.AssetId,
                                     UnitPrice = q.UnitPrice,
                                     Timestamp = q.CreatedAt
                                 })
                                 .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        if (asset is null || asset.Asset is null)
        {
            return Result.Failure(new ApiErrorResponse(HttpStatusCode.NotFound, $"Asset with code {code} not found."));
        }

        if (asset.LastQuotation is null)
        {
            return Result.Failure(new ApiErrorResponse(HttpStatusCode.NotFound, $"No quotations found for asset with code {code}."));
        }


        return Result.Success(asset.LastQuotation);
    }
}
