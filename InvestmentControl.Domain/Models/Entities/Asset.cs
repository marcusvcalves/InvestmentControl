using InvestmentControl.Domain.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace InvestmentControl.Domain.Models.Entities;

public class Asset : BaseEntity
{
    [Required]
    public required string Code { get; set; }

    [Required]
    public required string Name { get; set; }

    public virtual ICollection<Operation> Operations { get; set; } = new List<Operation>();

    public virtual ICollection<Position> Positions { get; set; } = new List<Position>();

    public virtual ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();

    public static Asset Create(string code, string name)
    {
        return new Asset
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = name
        };
    }
}
