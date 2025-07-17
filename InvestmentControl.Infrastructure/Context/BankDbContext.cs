using InvestmentControl.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestmentControl.Infrastructure.Context;

public partial class BankDbContext : DbContext
{
    public BankDbContext(DbContextOptions<BankDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Asset> Assets { get; set; }

    public virtual DbSet<Operation> Operations { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Quotation> Quotations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
