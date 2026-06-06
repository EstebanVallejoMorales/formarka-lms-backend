using FormarkaLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormarkaLms.Infrastructure.Persistence.Configurations;

public class QuizOptionConfiguration : IEntityTypeConfiguration<QuizOption>
{
    public void Configure(EntityTypeBuilder<QuizOption> builder)
    {
        builder.HasKey(qo => qo.Id);
        builder.Property(qo => qo.Id).ValueGeneratedOnAdd();

        builder.HasOne(qo => qo.Question)
            .WithMany(qq => qq.Options)
            .HasForeignKey(qo => qo.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
