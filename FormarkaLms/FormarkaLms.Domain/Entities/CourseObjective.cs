using FormarkaLms.Domain.Common;

namespace FormarkaLms.Domain.Entities;

public class CourseObjective : BaseEntity
{
    public int CourseId { get; set; }
    public string Text { get; set; } = default!;

    // Navigation Properties
    public virtual Course Course { get; set; } = default!;
}
