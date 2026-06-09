using MediatR;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Domain.Entities;
using FormarkaLms.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Quizzes.DTOs;

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
            Level = MapCourseLevel(request.Level),
            TotalHours = request.TotalHours,
            InstructorId = request.InstructorId,
            CreatedAt = DateTime.UtcNow
        };

        int mOrder = 1;
        foreach (var mDto in request.Modules)
        {
            var module = new FormarkaLms.Domain.Entities.Module
            {
                Title = mDto.Title,
                Order = mOrder++,
                CreatedAt = DateTime.UtcNow
            };

            int lOrder = 1;
            foreach (var lDto in mDto.Lessons)
            {
                var lType = Enum.Parse<LessonType>(lDto.Type, true);
                var lesson = new Lesson
                {
                    Title = lDto.Title,
                    Type = lType,
                    ContentUrl = lDto.ContentUrl,
                    Duration = ParseDuration(lDto.Duration),
                    Order = lOrder++,
                    CreatedAt = DateTime.UtcNow
                };

                foreach (var rDto in lDto.Resources)
                {
                    lesson.Resources.Add(new Resource
                    {
                        Title = rDto.Title,
                        Url = rDto.Url,
                        Type = Enum.Parse<ResourceType>(rDto.Type, true),
                        CreatedAt = DateTime.UtcNow
                    });
                }

                if (lType == LessonType.Quiz && lDto.Quiz != null)
                {
                    var quiz = new Quiz
                    {
                        Title = lDto.Quiz.Title,
                        Description = lDto.Quiz.Description,
                        PassingScore = lDto.Quiz.PassingScore,
                        CreatedAt = DateTime.UtcNow
                    };

                    int qOrder = 1;
                    foreach (var qDto in lDto.Quiz.Questions)
                    {
                        var question = new QuizQuestion
                        {
                            Text = qDto.Text,
                            QuestionType = Enum.Parse<QuestionType>(qDto.Type, true),
                            Points = qDto.Points,
                            Order = qOrder++,
                            CreatedAt = DateTime.UtcNow
                        };

                        foreach (var oDto in qDto.Options)
                        {
                            question.Options.Add(new QuizOption
                            {
                                Text = oDto.Text,
                                IsCorrect = oDto.IsCorrect,
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                        quiz.Questions.Add(question);
                    }
                    lesson.Quizzes.Add(quiz);
                }

                module.Lessons.Add(lesson);
            }

            course.Modules.Add(module);
        }

        _context.Courses.Add(course);
        await _context.SaveChangesAsync(cancellationToken);

        return course.Id;
    }

    private CourseLevel MapCourseLevel(string level)
    {
        var normalized = level.ToLower().Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u");
        return normalized switch
        {
            "basico" => CourseLevel.Basico,
            "intermedio" => CourseLevel.Intermedio,
            "avanzado" => CourseLevel.Avanzado,
            _ => CourseLevel.Basico
        };
    }

    private int ParseDuration(string? duration)
    {
        if (string.IsNullOrEmpty(duration)) return 0;
        if (int.TryParse(duration, out int mins)) return mins;
        
        if (duration.Contains(":"))
        {
            var parts = duration.Split(':');
            if (parts.Length >= 1 && int.TryParse(parts[0], out int m)) return m;
        }

        return 0;
    }
}
