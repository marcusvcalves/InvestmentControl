using InvestmentControl.Domain.Models.Entities.Base;
using NodaTime;
using System.ComponentModel.DataAnnotations;

namespace InvestmentControl.Domain.Models.Entities;

public class Operation : BaseEntity
{
    [Required]
    public required Guid UserId { get; set; }

    [Required]
    public required Guid AssetId { get; set; }

    [Required]
    public required int Quantity { get; set; }

    [Required]
    public required decimal UnitPrice { get; set; }

    [Required]
    public required int OperationType { get; set; }

    [Required]
    public required decimal Brokerage { get; set; }

    public Instant CreatedAt { get; set; }

    public virtual Asset Asset { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
