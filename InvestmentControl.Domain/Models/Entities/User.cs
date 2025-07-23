using InvestmentControl.Domain.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace InvestmentControl.Domain.Models.Entities;

public class User : BaseEntity
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Email { get; set; }

    [Required]
    public required decimal BrokeragePercentage { get; set; }

    public virtual ICollection<Operation> Operations { get; set; } = new List<Operation>();

    public virtual ICollection<Position> Positions { get; set; } = new List<Position>();

    public static User Create(Guid guid, string name, string email, decimal brokeragePercentage)
    {
        return new User
        {
            Id = guid,
            Name = name,
            Email = email,
            BrokeragePercentage = brokeragePercentage
        };
    }

    public static User Create(string name, string email, decimal brokeragePercentage)
    {
        return new User
        {
            Id = new Guid(),
            Name = name,
            Email = email,
            BrokeragePercentage = brokeragePercentage
        };
    }
}
