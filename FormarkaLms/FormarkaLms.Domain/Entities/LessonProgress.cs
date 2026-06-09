namespace FormarkaLms.Domain.Entities;

public class LessonProgress
{
    public string StudentId { get; set; } = default!;
    public string UserId => StudentId;
    public int LessonId { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime LastAccessed { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual Student Student { get; set; } = default!;
    public virtual Lesson Lesson { get; set; } = default!;
}
