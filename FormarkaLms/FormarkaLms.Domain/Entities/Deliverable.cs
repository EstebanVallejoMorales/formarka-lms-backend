using FormarkaLms.Domain.Common;
using FormarkaLms.Domain.Enums;

namespace FormarkaLms.Domain.Entities;

public class Deliverable : BaseEntity
{
    public string StudentId { get; set; } = default!;
    public int LessonId { get; set; }
    public string ContentUrl { get; set; } = default!;
    public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;
    public decimal? Grade { get; set; }
    public string? Feedback { get; set; }
    public DeliverableStatus Status { get; set; } = DeliverableStatus.Pending;

    // Navigation Properties
    public virtual Student Student { get; set; } = default!;
    public virtual Lesson Lesson { get; set; } = default!;
}
