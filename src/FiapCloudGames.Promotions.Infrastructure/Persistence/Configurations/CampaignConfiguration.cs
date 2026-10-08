using FiapCloudGames.Promotions.Domain.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiapCloudGames.Promotions.Infrastructure.Persistence.Configurations;

internal sealed class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.ToTable("campaigns");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(c => c.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(c => c.DiscountType).HasColumnName("discount_type")
            .HasConversion<int>()
            .HasDefaultValue(DiscountType.Percentage)
            .HasSentinel(DiscountType.Undefined)
            .IsRequired();
        builder.Property(c => c.DiscountValue).HasColumnName("discount_value")
            .HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.StartDateUtc).HasColumnName("start_date_utc").IsRequired();
        builder.Property(c => c.EndDateUtc).HasColumnName("end_date_utc").IsRequired();
        builder.Property(c => c.GameId).HasColumnName("game_id");
        builder.Property(c => c.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(c => c.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();

        builder.HasIndex(c => new { c.GameId, c.IsActive, c.StartDateUtc, c.EndDateUtc })
            .HasDatabaseName("ix_campaigns_eligibility");
    }
}
