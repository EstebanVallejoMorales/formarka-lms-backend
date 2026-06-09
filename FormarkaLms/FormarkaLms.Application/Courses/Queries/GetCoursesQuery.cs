using MediatR;
using FormarkaLms.Domain.Enums;

namespace FormarkaLms.Application.Courses.Queries;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ThumbnailUrl { get; set; }
    public string? Category { get; set; }
    public string Level { get; set; } = default!;
    public string InstructorName { get; set; } = default!;
    public string InstructorId { get; set; } = default!;
    public int TotalHours { get; set; }
}

public record GetCoursesQuery(string? InstructorId = null) : IRequest<List<CourseDto>>;
