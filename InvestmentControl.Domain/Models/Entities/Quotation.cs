using InvestmentControl.Domain.Models.Entities.Base;
using NodaTime;
using System.ComponentModel.DataAnnotations;

namespace InvestmentControl.Domain.Models.Entities;

public class Quotation : BaseEntity
{
    [Required]
    public required Guid AssetId { get; set; }

    public decimal? UnitPrice { get; set; }

    public Instant CreatedAt { get; set; }

    public virtual Asset Asset { get; set; } = null!;

    public static Quotation Create(Guid assetId, decimal? unitPrice)
    {
        return new Quotation
        {
            Id = Guid.NewGuid(),
            AssetId = assetId,
            UnitPrice = unitPrice,
            CreatedAt = SystemClock.Instance.GetCurrentInstant()
        };
    }
}
