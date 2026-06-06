namespace FormarkaLms.Domain.Entities;

public class Student
{
    public string Id { get; set; } = default!; // FK to User.Id
    public string? IdentificationNumber { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? AcademicLevel { get; set; }
    public string? Biography { get; set; }
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual User User { get; set; } = default!;
    public virtual ICollection<Entrepreneurship> Entrepreneurships { get; set; } = new List<Entrepreneurship>();
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
    public virtual ICollection<Deliverable> Deliverables { get; set; } = new List<Deliverable>();
    public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
}
