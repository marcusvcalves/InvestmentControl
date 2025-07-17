using InvestmentControl.Domain.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace InvestmentControl.Domain.Models.Entities;

public class Position : BaseEntity
{
    [Required]
    public required Guid UserId { get; set; }

    [Required]
    public required Guid AssetId { get; set; }

    [Required]
    public required int Quantity { get; set; }

    [Required]
    public required decimal MediumPrice { get; set; }

    [Required]
    public required decimal ProfitLoss { get; set; }

    public virtual Asset Asset { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
