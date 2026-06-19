using MediatR;
using FormarkaLms.Application.Quizzes.Queries;

namespace FormarkaLms.Application.Courses.Queries;

public class CourseDetailDto : CourseDto
{
    public bool IsEnrolled { get; set; }
    public int? LastVisitedLessonId { get; set; }
    public string? LongDescription { get; set; }
    public List<string> Objectives { get; set; } = new();
    public List<FeatureDto> Features { get; set; } = new();
    public List<ModuleDto> Modules { get; set; } = new();
}

public class FeatureDto
{
    public int Id { get; set; }
    public string Icon { get; set; } = default!;
    public string Text { get; set; } = default!;
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
    public string? Description { get; set; }
    public string? ContentUrl { get; set; }
    public string? Duration { get; set; }
    public bool IsCompleted { get; set; }
    public QuizDto? Quiz { get; set; }
}

public record GetCourseByIdQuery(int Id, string? UserId = null) : IRequest<CourseDetailDto?>;
