using MediatR;
using FormarkaLms.Application.Quizzes.Queries;

namespace FormarkaLms.Application.Courses.Queries;

public class CourseDetailDto : CourseDto
{
    public bool IsEnrolled { get; set; }
    public int? LastVisitedLessonId { get; set; }
    public List<ModuleDto> Modules { get; set; } = new();
}

public class ModuleDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public List<LessonDto> Lessons { get; set; } = new();
}

public class LessonDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string? ContentUrl { get; set; }
    public string? Duration { get; set; }
    public bool IsCompleted { get; set; }
    public QuizDto? Quiz { get; set; }
}

public record GetCourseByIdQuery(int Id, string? UserId = null) : IRequest<CourseDetailDto?>;
