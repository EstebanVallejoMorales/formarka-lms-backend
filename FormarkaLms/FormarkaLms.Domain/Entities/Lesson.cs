using FormarkaLms.Domain.Common;
using FormarkaLms.Domain.Enums;

namespace FormarkaLms.Domain.Entities;

public class Lesson : BaseEntity
{
    public int ModuleId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public LessonType Type { get; set; }
    public string? ContentUrl { get; set; }
    public int Duration { get; set; } // In minutes probably
    public int Order { get; set; }
    public bool IsMandatory { get; set; } = true;

    // Navigation Properties
    public virtual Module Module { get; set; } = default!;
    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();
    public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
    public virtual ICollection<Deliverable> Deliverables { get; set; } = new List<Deliverable>();
    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
