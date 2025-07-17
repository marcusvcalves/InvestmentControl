using InvestmentControl.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvestmentControl.Infrastructure.Configurations.Entities;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.HasKey(e => e.Id).HasName("positions_pkey");

        builder.ToTable("positions");

        builder.HasIndex(e => e.AssetId, "idx_positions_asset_id");

        builder.HasIndex(e => e.UserId, "idx_positions_user_id");

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");
        builder.Property(e => e.AssetId).HasColumnName("asset_id");
        builder.Property(e => e.MediumPrice).HasColumnName("medium_price");
        builder.Property(e => e.ProfitLoss).HasColumnName("profit_loss");
        builder.Property(e => e.Quantity).HasColumnName("quantity");
        builder.Property(e => e.UserId).HasColumnName("user_id");

        builder.HasOne(d => d.Asset).WithMany(p => p.Positions)
            .HasForeignKey(d => d.AssetId)
            .HasConstraintName("fk_asset_positions");

        builder.HasOne(d => d.User).WithMany(p => p.Positions)
            .HasForeignKey(d => d.UserId)
            .HasConstraintName("fk_user_positions");
    }
}