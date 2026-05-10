namespace FormarkaLMS.Services.Learning.Domain.Entities;

public class Enrollment
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime EnrolledAt { get; set; }
    public bool IsCompleted { get; set; }
    public double ProgressPercentage { get; set; }
}

public class LessonProgress
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid LessonId { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}
