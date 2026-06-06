using FormarkaLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormarkaLms.Infrastructure.Persistence.Configurations;

public class EntrepreneurshipConfiguration : IEntityTypeConfiguration<Entrepreneurship>
{
    public void Configure(EntityTypeBuilder<Entrepreneurship> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Name).IsRequired().HasMaxLength(255);

        builder.HasOne(e => e.Student)
            .WithMany(s => s.Entrepreneurships)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
