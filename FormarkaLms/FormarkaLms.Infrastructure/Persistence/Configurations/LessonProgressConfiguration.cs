using FormarkaLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormarkaLms.Infrastructure.Persistence.Configurations;

public class LessonProgressConfiguration : IEntityTypeConfiguration<LessonProgress>
{
    public void Configure(EntityTypeBuilder<LessonProgress> builder)
    {
        // Composite Key
        builder.HasKey(lp => new { lp.StudentId, lp.LessonId });

        builder.HasOne(lp => lp.Student)
            .WithMany(s => s.LessonProgresses)
            .HasForeignKey(lp => lp.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(lp => lp.Lesson)
            .WithMany(l => l.LessonProgresses)
            .HasForeignKey(lp => lp.LessonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
