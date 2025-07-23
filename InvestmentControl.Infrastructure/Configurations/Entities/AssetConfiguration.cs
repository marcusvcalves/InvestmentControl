using InvestmentControl.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvestmentControl.Infrastructure.Configurations.Entities;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.HasKey(e => e.Id)
               .HasName("assets_pkey");

        builder.ToTable("assets");

        builder.HasIndex(e => e.Code)
               .HasDatabaseName("idx_assets_code");

        builder.Property(e => e.Id)
               .ValueGeneratedNever()
               .HasColumnName("id");

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnType("citext")
            .HasColumnName("code");


        builder.Property(e => e.Name)
               .HasMaxLength(50)
               .HasColumnName("name");
    }
}