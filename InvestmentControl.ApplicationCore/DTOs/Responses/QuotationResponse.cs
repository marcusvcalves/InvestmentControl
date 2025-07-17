using NodaTime;

namespace InvestmentControl.ApplicationCore.DTOs.Responses;

public class QuotationResponse
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public decimal? UnitPrice { get; set; }
    public Instant CreatedAt { get; set; }
}
