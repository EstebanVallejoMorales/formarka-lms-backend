using FormarkaLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormarkaLms.Infrastructure.Persistence.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        builder.Property(c => c.Title).IsRequired().HasMaxLength(255);

        // Map LearningObjectives (List<string>) to JSONB in Postgres
        builder.Property(c => c.LearningObjectives)
            .HasColumnType("jsonb");

        // Map Features (List<CourseFeature>) to JSONB in Postgres
        builder.Property(c => c.Features)
            .HasColumnType("jsonb");

        builder.HasOne(c => c.Instructor)
            .WithMany(i => i.Courses)
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
