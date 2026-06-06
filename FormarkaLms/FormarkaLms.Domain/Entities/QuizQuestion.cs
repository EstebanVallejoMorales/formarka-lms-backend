using FormarkaLms.Domain.Common;
using FormarkaLms.Domain.Enums;

namespace FormarkaLms.Domain.Entities;

public class QuizQuestion : BaseEntity
{
    public int QuizId { get; set; }
    public string Text { get; set; } = default!;
    public QuestionType QuestionType { get; set; }
    public int Points { get; set; }
    public int Order { get; set; }

    // Navigation Properties
    public virtual Quiz Quiz { get; set; } = default!;
    public virtual ICollection<QuizOption> Options { get; set; } = new List<QuizOption>();
}
