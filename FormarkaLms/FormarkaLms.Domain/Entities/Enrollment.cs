using FormarkaLms.Domain.Enums;

namespace FormarkaLms.Domain.Entities;

public class Enrollment
{
    public string StudentId { get; set; } = default!;
    public int CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public int Progress { get; set; }
    public decimal Grade { get; set; }
    public bool IsCompleted { get; set; }

    // Navigation Properties
    public virtual Student Student { get; set; } = default!;
    public virtual Course Course { get; set; } = default!;
}
