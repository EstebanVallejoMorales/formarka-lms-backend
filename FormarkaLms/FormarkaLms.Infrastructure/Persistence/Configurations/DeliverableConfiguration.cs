using FormarkaLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormarkaLms.Infrastructure.Persistence.Configurations;

public class DeliverableConfiguration : IEntityTypeConfiguration<Deliverable>
{
    public void Configure(EntityTypeBuilder<Deliverable> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedOnAdd();

        builder.HasOne(d => d.Student)
            .WithMany(s => s.Deliverables)
            .HasForeignKey(d => d.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Lesson)
            .WithMany(l => l.Deliverables)
            .HasForeignKey(d => d.LessonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
