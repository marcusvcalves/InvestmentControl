using InvestmentControl.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvestmentControl.Infrastructure.Configurations.Entities
{
    public class QuotationConfiguration : IEntityTypeConfiguration<Quotation>
    {
        public void Configure(EntityTypeBuilder<Quotation> builder)
        {
            builder.HasKey(e => e.Id).HasName("quotations_pkey");

            builder.ToTable("quotations");

            builder.HasIndex(e => e.AssetId, "idx_quotations_asset_id");

            builder.HasIndex(e => e.CreatedAt, "idx_quotations_created_at");

            builder.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            builder.Property(e => e.AssetId).HasColumnName("asset_id");
            builder.Property(e => e.CreatedAt).HasColumnName("created_at");
            builder.Property(e => e.UnitPrice).HasColumnName("unit_price");

            builder.HasOne(d => d.Asset).WithMany(p => p.Quotations)
                .HasForeignKey(d => d.AssetId)
                .HasConstraintName("fk_asset_quotations");
        }
    }
}