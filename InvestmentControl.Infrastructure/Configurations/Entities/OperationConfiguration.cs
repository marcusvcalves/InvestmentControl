using InvestmentControl.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvestmentControl.Infrastructure.Configurations.Entities;

public class OperationConfiguration : IEntityTypeConfiguration<Operation>
{
    public void Configure(EntityTypeBuilder<Operation> builder)
    {
        builder.HasKey(e => e.Id).HasName("operations_pkey");

        builder.ToTable("operations");

        builder.HasIndex(e => e.AssetId, "idx_operations_asset_id");

        builder.HasIndex(e => e.CreatedAt, "idx_operations_created_at");

        builder.HasIndex(e => e.UserId, "idx_operations_user_id");

        builder.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

        builder.Property(e => e.AssetId).HasColumnName("asset_id");
        builder.Property(e => e.Brokerage).HasColumnName("brokerage");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");
        builder.Property(e => e.OperationType).HasColumnName("operation_type");
        builder.Property(e => e.Quantity).HasColumnName("quantity");
        builder.Property(e => e.UnitPrice).HasColumnName("unit_price");
        builder.Property(e => e.UserId).HasColumnName("user_id");

        builder.HasOne(d => d.Asset).WithMany(p => p.Operations)
                .HasForeignKey(d => d.AssetId)
                .HasConstraintName("fk_asset_operations");

        builder.HasOne(d => d.User).WithMany(p => p.Operations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_operations");
    }
}