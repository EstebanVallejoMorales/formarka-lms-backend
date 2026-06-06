using FormarkaLms.Domain.Common;

namespace FormarkaLms.Domain.Entities;

public class QuizAttempt : BaseEntity
{
    public int QuizId { get; set; }
    public string StudentId { get; set; } = default!;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }
    public decimal FinalScore { get; set; }

    // Navigation Properties
    public virtual Quiz Quiz { get; set; } = default!;
    public virtual Student Student { get; set; } = default!;
}
