using FormarkaLms.Domain.Common;

namespace FormarkaLms.Domain.Entities;

public class Quiz : BaseEntity
{
    public int LessonId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public decimal PassingScore { get; set; }
    public int TotalPoints { get; set; }

    // Navigation Properties
    public virtual Lesson Lesson { get; set; } = default!;
    public virtual ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
    public virtual ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
}
