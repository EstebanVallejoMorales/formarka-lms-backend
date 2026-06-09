using FormarkaLms.Application.Quizzes.DTOs;

namespace FormarkaLms.Application.Courses.Commands;

public record ModuleCommandDto
{
    public int? Id { get; set; }
    public string Title { get; set; } = default!;
    public List<LessonCommandDto> Lessons { get; set; } = new();
}

public record LessonCommandDto
{
    public int? Id { get; set; }
    public string Title { get; set; } = default!;
    public string Type { get; set; } = "video";
    public string? ContentUrl { get; set; }
    public string? Duration { get; set; }
    public List<ResourceCommandDto> Resources { get; set; } = new();
    public QuizAdminDto? Quiz { get; set; }
}

public record ResourceCommandDto
{
    public int? Id { get; set; }
    public string Title { get; set; } = default!;
    public string Url { get; set; } = default!;
    public string Type { get; set; } = "pdf";
}
