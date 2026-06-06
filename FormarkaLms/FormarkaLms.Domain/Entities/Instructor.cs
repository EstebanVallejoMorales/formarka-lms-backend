namespace FormarkaLms.Domain.Entities;

public class Instructor
{
    public string Id { get; set; } = default!; // FK to User.Id
    public string? ProfessionalTitle { get; set; }
    public string? Biography { get; set; }
    public int YearsOfExperience { get; set; }
    public string? LinkedInUrl { get; set; }

    // Navigation Properties
    public virtual User User { get; set; } = default!;
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
