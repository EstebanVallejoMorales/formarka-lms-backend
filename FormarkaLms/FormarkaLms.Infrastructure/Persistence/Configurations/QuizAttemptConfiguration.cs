using FormarkaLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormarkaLms.Infrastructure.Persistence.Configurations;

public class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
    public void Configure(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.HasKey(qa => qa.Id);
        builder.Property(qa => qa.Id).ValueGeneratedOnAdd();

        builder.HasOne(qa => qa.Quiz)
            .WithMany(q => q.Attempts)
            .HasForeignKey(qa => qa.QuizId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(qa => qa.Student)
            .WithMany(s => s.QuizAttempts)
            .HasForeignKey(qa => qa.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
