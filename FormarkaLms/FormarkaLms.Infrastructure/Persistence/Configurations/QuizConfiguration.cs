using FormarkaLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormarkaLms.Infrastructure.Persistence.Configurations;

public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Id).ValueGeneratedOnAdd();

        builder.HasOne(q => q.Lesson)
            .WithMany(l => l.Quizzes)
            .HasForeignKey(q => q.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
