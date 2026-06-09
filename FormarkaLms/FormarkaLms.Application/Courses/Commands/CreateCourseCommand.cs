using MediatR;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Domain.Entities;
using FormarkaLms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLms.Application.Courses.Commands;

public record CreateCourseCommand : IRequest<int>
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ThumbnailUrl { get; set; }
    public string? Category { get; set; }
    public string Level { get; set; } = "básico";
    public int TotalHours { get; set; }
    public string InstructorId { get; set; } = default!;
    public List<ModuleCommandDto> Modules { get; set; } = new();
}

public record ModuleCommandDto
{
    public string Title { get; set; } = default!;
    public List<LessonCommandDto> Lessons { get; set; } = new();
}

public record LessonCommandDto
{
    public string Title { get; set; } = default!;
    public string Type { get; set; } = "video";
    public string? ContentUrl { get; set; }
    public string? Duration { get; set; }
}

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateCourseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = new Course
        {
            Title = request.Title,
            Description = request.Description,
            ThumbnailUrl = request.ThumbnailUrl,
            Category = request.Category,
            Level = Enum.Parse<CourseLevel>(request.Level, true),
            TotalHours = request.TotalHours,
            InstructorId = request.InstructorId,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var mDto in request.Modules)
        {
            var module = new FormarkaLms.Domain.Entities.Module
            {
                Title = mDto.Title,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var lDto in mDto.Lessons)
            {
                module.Lessons.Add(new Lesson
                {
                    Title = lDto.Title,
                    Type = Enum.Parse<LessonType>(lDto.Type, true),
                    ContentUrl = lDto.ContentUrl,
                    Duration = ParseDuration(lDto.Duration),
                    CreatedAt = DateTime.UtcNow
                });
            }

            course.Modules.Add(module);
        }

        _context.Courses.Add(course);
        await _context.SaveChangesAsync(cancellationToken);

        return course.Id;
    }

    private int ParseDuration(string? duration)
    {
        if (string.IsNullOrEmpty(duration)) return 0;
        if (int.TryParse(duration, out int mins)) return mins;
        
        // Handle "MM:SS"
        if (duration.Contains(":"))
        {
            var parts = duration.Split(':');
            if (parts.Length >= 1 && int.TryParse(parts[0], out int m)) return m;
        }

        return 0;
    }
}
