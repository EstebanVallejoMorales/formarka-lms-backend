using FormarkaLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormarkaLms.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.HasIndex(u => u.Email).IsUnique();
        
        builder.HasOne(u => u.Student)
            .WithOne(s => s.User)
            .HasForeignKey<Student>(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.Instructor)
            .WithOne(i => i.User)
            .HasForeignKey<Instructor>(i => i.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
