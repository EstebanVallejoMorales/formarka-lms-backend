using FormarkaLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormarkaLms.Infrastructure.Persistence.Configurations;

public class CourseFeatureConfiguration : IEntityTypeConfiguration<CourseFeature>
{
    public void Configure(EntityTypeBuilder<CourseFeature> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Text).IsRequired().HasMaxLength(255);
        builder.Property(f => f.Icon).HasMaxLength(50);
    }
}
