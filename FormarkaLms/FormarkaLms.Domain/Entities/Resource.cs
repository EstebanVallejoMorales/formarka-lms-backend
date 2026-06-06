using FormarkaLms.Domain.Common;
using FormarkaLms.Domain.Enums;

namespace FormarkaLms.Domain.Entities;

public class Resource : BaseEntity
{
    public int LessonId { get; set; }
    public string Title { get; set; } = default!;
    public string Url { get; set; } = default!;
    public ResourceType Type { get; set; }

    // Navigation Properties
    public virtual Lesson Lesson { get; set; } = default!;
}
