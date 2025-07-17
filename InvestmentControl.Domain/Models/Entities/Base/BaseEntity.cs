using System.ComponentModel.DataAnnotations;

namespace InvestmentControl.Domain.Models.Entities.Base;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }
}
