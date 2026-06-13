using MediatR;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Domain.Entities;
using FormarkaLms.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Quizzes.DTOs;

namespace FormarkaLms.Application.Courses.Commands;

public record UpdateCourseCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? LongDescription { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Category { get; set; }
    public string Level { get; set; } = "básico";
    public int TotalHours { get; set; }
    public bool IsFree { get; set; } = false;
    public string? InstructorId { get; set; }
    public List<CourseObjectiveCommandDto> Objectives { get; set; } = new();
    public List<CourseFeatureCommandDto> Features { get; set; } = new();
    public List<ModuleCommandDto> Modules { get; set; } = new();
}

public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateCourseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .Include(c => c.LearningObjectives)
            .Include(c => c.Features)
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
                    .ThenInclude(l => l.Resources)
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
                    .ThenInclude(l => l.Quizzes)
                        .ThenInclude(q => q.Questions)
                            .ThenInclude(qs => qs.Options)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (course == null) return false;

        // Ensure Instructor exists if ID is provided
        if (!string.IsNullOrEmpty(request.InstructorId))
        {
            var instructorExists = await _context.Instructors.AnyAsync(i => i.Id == request.InstructorId, cancellationToken);
            if (!instructorExists)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.InstructorId, cancellationToken);
                if (user != null)
                {
                    _context.Instructors.Add(new Instructor
                    {
                        Id = request.InstructorId,
                        ProfessionalTitle = user.Specialty ?? "Instructor"
                    });
                }
            }
        }

        // 1. Update general course info
        course.Title = request.Title;
        course.Description = request.Description;
        course.LongDescription = request.LongDescription;
        course.ThumbnailUrl = request.ThumbnailUrl;
        course.Category = request.Category;
        course.Level = MapCourseLevel(request.Level);
        course.TotalHours = request.TotalHours;
        course.IsFree = request.IsFree;
        if (!string.IsNullOrEmpty(request.InstructorId)) course.InstructorId = request.InstructorId;
        course.UpdatedAt = DateTime.UtcNow;

        // Synchronize Objectives
        SyncObjectives(course, request.Objectives);

        // Synchronize Features
        SyncFeatures(course, request.Features);

        // 2. Synchronize Modules
        var existingModules = course.Modules.ToList();
        var incomingModuleIds = request.Modules.Where(m => m.Id.HasValue).Select(m => m.Id!.Value).ToList();

        foreach (var existingModule in existingModules)
        {
            if (!incomingModuleIds.Contains(existingModule.Id))
            {
                _context.Modules.Remove(existingModule);
            }
        }

        int mOrder = 1;
        foreach (var mDto in request.Modules)
        {
            var module = mDto.Id.HasValue 
                ? existingModules.FirstOrDefault(em => em.Id == mDto.Id.Value)
                : null;

            if (module != null)
            {
                module.Title = mDto.Title;
                module.Order = mOrder++;
                module.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                module = new FormarkaLms.Domain.Entities.Module
                {
                    Title = mDto.Title,
                    Order = mOrder++,
                    CreatedAt = DateTime.UtcNow,
                    CourseId = course.Id
                };
                course.Modules.Add(module);
            }

            // 3. Synchronize Lessons
            var existingLessons = module.Lessons.ToList();
            var incomingLessonIds = mDto.Lessons.Where(l => l.Id.HasValue).Select(l => l.Id!.Value).ToList();

            foreach (var existingLesson in existingLessons)
            {
                if (!incomingLessonIds.Contains(existingLesson.Id))
                {
                    _context.Lessons.Remove(existingLesson);
                }
            }

            int lOrder = 1;
            foreach (var lDto in mDto.Lessons)
            {
                var lesson = lDto.Id.HasValue
                    ? existingLessons.FirstOrDefault(el => el.Id == lDto.Id.Value)
                    : null;

                var lType = Enum.Parse<LessonType>(lDto.Type, true);

                if (lesson != null)
                {
                    lesson.Title = lDto.Title;
                    lesson.Type = lType;
                    lesson.ContentUrl = lDto.ContentUrl;
                    lesson.Duration = ParseDuration(lDto.Duration);
                    lesson.Order = lOrder++;
                    lesson.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    lesson = new Lesson
                    {
                        Title = lDto.Title,
                        Type = lType,
                        ContentUrl = lDto.ContentUrl,
                        Duration = ParseDuration(lDto.Duration),
                        Order = lOrder++,
                        CreatedAt = DateTime.UtcNow,
                        ModuleId = module.Id
                    };
                    module.Lessons.Add(lesson);
                }

                // 4. Synchronize Resources
                SyncResources(lesson, lDto.Resources);

                // 5. Synchronize Quizzes
                if (lType == LessonType.Quiz && lDto.Quiz != null)
                {
                    SyncQuiz(lesson, lDto.Quiz);
                }
                else if (lesson.Quizzes.Any())
                {
                    // If type changed or quiz removed, remove quizzes
                    foreach (var q in lesson.Quizzes.ToList())
                    {
                        _context.Quizzes.Remove(q);
                    }
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private void SyncObjectives(Course course, List<CourseObjectiveCommandDto> dtos)
    {
        var existing = course.LearningObjectives.ToList();
        var incomingIds = dtos.Where(o => o.Id.HasValue).Select(o => o.Id!.Value).ToList();

        foreach (var ex in existing)
        {
            if (!incomingIds.Contains(ex.Id)) _context.CourseObjectives.Remove(ex);
        }

        foreach (var dto in dtos)
        {
            var objective = dto.Id.HasValue ? existing.FirstOrDefault(o => o.Id == dto.Id.Value) : null;
            if (objective != null)
            {
                objective.Text = dto.Text;
                objective.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                course.LearningObjectives.Add(new CourseObjective
                {
                    Text = dto.Text,
                    CreatedAt = DateTime.UtcNow,
                    CourseId = course.Id
                });
            }
        }
    }

    private void SyncFeatures(Course course, List<CourseFeatureCommandDto> dtos)
    {
        var existing = course.Features.ToList();
        var incomingIds = dtos.Where(f => f.Id.HasValue).Select(f => f.Id!.Value).ToList();

        foreach (var ex in existing)
        {
            if (!incomingIds.Contains(ex.Id)) _context.CourseFeatures.Remove(ex);
        }

        foreach (var dto in dtos)
        {
            var feature = dto.Id.HasValue ? existing.FirstOrDefault(f => f.Id == dto.Id.Value) : null;
            if (feature != null)
            {
                feature.Icon = dto.Icon;
                feature.Text = dto.Text;
                feature.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                course.Features.Add(new CourseFeature
                {
                    Icon = dto.Icon,
                    Text = dto.Text,
                    CreatedAt = DateTime.UtcNow,
                    CourseId = course.Id
                });
            }
        }
    }

    private void SyncResources(Lesson lesson, List<ResourceCommandDto> dtos)
    {
        var existingResources = lesson.Resources.ToList();
        var incomingResourceIds = dtos.Where(r => r.Id.HasValue).Select(r => r.Id!.Value).ToList();

        foreach (var existingRes in existingResources)
        {
            if (!incomingResourceIds.Contains(existingRes.Id))
            {
                _context.Resources.Remove(existingRes);
            }
        }

        foreach (var rDto in dtos)
        {
            var resource = rDto.Id.HasValue
                ? existingResources.FirstOrDefault(er => er.Id == rDto.Id.Value)
                : null;

            var rType = Enum.Parse<ResourceType>(rDto.Type, true);

            if (resource != null)
            {
                resource.Title = rDto.Title;
                resource.Url = rDto.Url;
                resource.Type = rType;
                resource.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                resource = new Resource
                {
                    Title = rDto.Title,
                    Url = rDto.Url,
                    Type = rType,
                    CreatedAt = DateTime.UtcNow,
                    LessonId = lesson.Id
                };
                lesson.Resources.Add(resource);
            }
        }
    }

    private void SyncQuiz(Lesson lesson, QuizAdminDto dto)
    {
        var quiz = lesson.Quizzes.FirstOrDefault();
        if (quiz == null)
        {
            quiz = new Quiz
            {
                LessonId = lesson.Id,
                CreatedAt = DateTime.UtcNow
            };
            lesson.Quizzes.Add(quiz);
        }

        quiz.Title = dto.Title;
        quiz.Description = dto.Description;
        quiz.PassingScore = dto.PassingScore;
        quiz.UpdatedAt = DateTime.UtcNow;

        // Sync Questions
        var existingQuestions = quiz.Questions.ToList();
        var incomingQuestionIds = dto.Questions.Where(q => q.Id.HasValue).Select(q => q.Id!.Value).ToList();

        foreach (var eq in existingQuestions)
        {
            if (!incomingQuestionIds.Contains(eq.Id))
                _context.QuizQuestions.Remove(eq);
        }

        int qOrder = 1;
        foreach (var qDto in dto.Questions)
        {
            var question = qDto.Id.HasValue ? existingQuestions.FirstOrDefault(eq => eq.Id == qDto.Id.Value) : null;
            var qType = Enum.Parse<QuestionType>(qDto.Type, true);

            if (question != null)
            {
                question.Text = qDto.Text;
                question.QuestionType = qType;
                question.Points = qDto.Points;
                question.Order = qOrder++;
                question.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                question = new QuizQuestion
                {
                    Text = qDto.Text,
                    QuestionType = qType,
                    Points = qDto.Points,
                    Order = qOrder++,
                    CreatedAt = DateTime.UtcNow
                };
                quiz.Questions.Add(question);
            }

            // Sync Options
            var existingOptions = question.Options.ToList();
            var incomingOptionIds = qDto.Options.Where(o => o.Id.HasValue).Select(o => o.Id!.Value).ToList();

            foreach (var eo in existingOptions)
            {
                if (!incomingOptionIds.Contains(eo.Id))
                    _context.QuizOptions.Remove(eo);
            }

            foreach (var oDto in qDto.Options)
            {
                var option = oDto.Id.HasValue ? existingOptions.FirstOrDefault(eo => eo.Id == oDto.Id.Value) : null;
                if (option != null)
                {
                    option.Text = oDto.Text;
                    option.IsCorrect = oDto.IsCorrect;
                    option.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    option = new QuizOption
                    {
                        Text = oDto.Text,
                        IsCorrect = oDto.IsCorrect,
                        CreatedAt = DateTime.UtcNow
                    };
                    question.Options.Add(option);
                }
            }
        }
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
