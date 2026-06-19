using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Application.Quizzes.Queries;
using FormarkaLms.Domain.Enums;

namespace FormarkaLms.Application.Courses.Queries;

public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, CourseDetailDto?>
{
    private readonly IApplicationDbContext _context;

    public GetCourseByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CourseDetailDto?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .Include(c => c.Instructor)
            .ThenInclude(i => i.User)
            .Include(c => c.LearningObjectives)
            .Include(c => c.Features)
            .Include(c => c.Modules.OrderBy(m => m.Order))
            .ThenInclude(m => m.Lessons.OrderBy(l => l.Order))
            .ThenInclude(l => l.Quizzes)
            .ThenInclude(q => q.Questions.OrderBy(qs => qs.Order))
            .ThenInclude(qs => qs.Options)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (course == null) return null;

        var completedLessonIds = new HashSet<int>();
        var isEnrolled = false;
        int? lastVisitedLessonId = null;

        if (!string.IsNullOrEmpty(request.UserId))
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            var isStaff = user != null && (user.Role == UserRole.Admin || user.Role == UserRole.Teacher);

            isEnrolled = isStaff || await _context.Enrollments
                .AnyAsync(e => e.CourseId == request.Id && e.StudentId == request.UserId, cancellationToken);

            var userProgress = await _context.LessonProgresses
                .Where(lp => lp.StudentId == request.UserId && lp.Lesson.Module.CourseId == request.Id)
                .Select(lp => new { lp.LessonId, lp.IsCompleted, lp.LastAccessed })
                .ToListAsync(cancellationToken);

            completedLessonIds = userProgress
                .Where(lp => lp.IsCompleted)
                .Select(lp => lp.LessonId)
                .ToHashSet();

            lastVisitedLessonId = userProgress
                .OrderByDescending(lp => lp.LastAccessed)
                .Select(lp => (int?)lp.LessonId)
                .FirstOrDefault();
        }

        return new CourseDetailDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            ThumbnailUrl = course.ThumbnailUrl,
            Category = course.Category,
            Level = course.Level switch
            {
                CourseLevel.Basico => "básico",
                CourseLevel.Intermedio => "intermedio",
                CourseLevel.Avanzado => "avanzado",
                _ => "básico"
            },
            InstructorId = course.InstructorId,
            InstructorName = course?.Instructor?.User?.Name,
            TotalHours = course?.TotalHours,
            IsFree = course.IsFree,
            IsEnrolled = isEnrolled,
            LastVisitedLessonId = lastVisitedLessonId,
            LongDescription = course.LongDescription,
            Objectives = course.LearningObjectives.Select(o => o.Text).ToList(),
            Features = course.Features.Select(f => new FeatureDto
            {
                Id = f.Id,
                Icon = f.Icon,
                Text = f.Text
            }).ToList(),
            Modules = course.Modules.Select(m => new ModuleDto
            {
                Id = m.Id,
                Title = m.Title,
                Lessons = m.Lessons.Select(l => new LessonDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Type = l.Type.ToString().ToLower(),
                    Description = l.Description,
                    ContentUrl = isEnrolled ? l.ContentUrl : null,
                    Duration = l.Duration.ToString(),
                    IsCompleted = completedLessonIds.Contains(l.Id),
                    Quiz = l.Quizzes.Select(q => new QuizDto
                    {
                        Id = q.Id,
                        Title = q.Title,
                        Description = q.Description,
                        PassingScore = q.PassingScore,
                        Questions = q.Questions.Select(qs => new QuizQuestionDto
                        {
                            Id = qs.Id,
                            Text = qs.Text,
                            Type = qs.QuestionType.ToString().ToLower(),
                            Points = qs.Points,
                            Options = qs.Options.Select(o => new QuizOptionDto
                            {
                                Id = o.Id,
                                Text = o.Text,
                                IsCorrect = o.IsCorrect
                            }).ToList()
                        }).ToList()
                    }).FirstOrDefault()
                }).ToList()
            }).ToList()
        };
    }
}

