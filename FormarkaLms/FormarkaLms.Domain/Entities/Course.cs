using FormarkaLms.Domain.Common;
using FormarkaLms.Domain.Enums;

namespace FormarkaLms.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? LongDescription { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Category { get; set; }
    public CourseLevel Level { get; set; }
    public string InstructorId { get; set; } = default!;
    public int TotalHours { get; set; }
    public string? Status { get; set; }
    
    public virtual Instructor Instructor { get; set; } = default!;
    public virtual ICollection<CourseObjective> LearningObjectives { get; set; } = new List<CourseObjective>();
    public virtual ICollection<CourseFeature> Features { get; set; } = new List<CourseFeature>();
    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
