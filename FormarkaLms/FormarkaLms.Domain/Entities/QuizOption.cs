using FormarkaLms.Domain.Common;

namespace FormarkaLms.Domain.Entities;

public class QuizOption : BaseEntity
{
    public int QuestionId { get; set; }
    public string Text { get; set; } = default!;
    public bool IsCorrect { get; set; }

    // Navigation Properties
    public virtual QuizQuestion Question { get; set; } = default!;
}
