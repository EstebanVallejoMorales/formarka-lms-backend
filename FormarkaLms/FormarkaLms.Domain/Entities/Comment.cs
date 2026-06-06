using FormarkaLms.Domain.Common;

namespace FormarkaLms.Domain.Entities;

public class Comment : BaseEntity
{
    public int LessonId { get; set; }
    public string UserId { get; set; } = default!;
    public string Content { get; set; } = default!;
    public int Likes { get; set; }
    public int? ParentId { get; set; }

    // Navigation Properties
    public virtual Lesson Lesson { get; set; } = default!;
    public virtual User User { get; set; } = default!;
    public virtual Comment? Parent { get; set; }
    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
}
