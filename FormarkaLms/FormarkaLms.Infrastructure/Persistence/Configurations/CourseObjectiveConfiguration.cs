using FormarkaLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormarkaLms.Infrastructure.Persistence.Configurations;

public class CourseObjectiveConfiguration : IEntityTypeConfiguration<CourseObjective>
{
    public void Configure(EntityTypeBuilder<CourseObjective> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Text).IsRequired().HasMaxLength(500);
    }
}
