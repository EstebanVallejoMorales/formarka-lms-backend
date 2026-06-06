using FormarkaLms.Domain.Common;

namespace FormarkaLms.Domain.Entities;

public class Module : BaseEntity
{
    public int CourseId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public int Order { get; set; }

    // Navigation Properties
    public virtual Course Course { get; set; } = default!;
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
