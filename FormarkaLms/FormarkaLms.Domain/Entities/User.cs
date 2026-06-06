using FormarkaLms.Domain.Enums;

namespace FormarkaLms.Domain.Entities;

public class User
{
    public string Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public UserRole Role { get; set; }
    public string? Specialty { get; set; } // Only for instructors
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public virtual Student? Student { get; set; }
    public virtual Instructor? Instructor { get; set; }
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
