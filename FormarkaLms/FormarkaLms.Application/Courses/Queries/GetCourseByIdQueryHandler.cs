using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Application.Quizzes.Queries;

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
            isEnrolled = await _context.Enrollments
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
            Level = course.Level.ToString().ToLower(),
            InstructorId = course.InstructorId,
            InstructorName = course.Instructor.User.Name,
            TotalHours = course.TotalHours,
            IsEnrolled = isEnrolled,
            LastVisitedLessonId = lastVisitedLessonId,
            Modules = course.Modules.Select(m => new ModuleDto
            {
                Id = m.Id,
                Title = m.Title,
                Lessons = m.Lessons.Select(l => new LessonDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Type = l.Type.ToString().ToLower(),
                    ContentUrl = l.ContentUrl,
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
                            Options = qs.Options.Select(o => new QuizOptionDto
                            {
                                Id = o.Id,
                                Text = o.Text
                            }).ToList()
                        }).ToList()
                    }).FirstOrDefault()
                }).ToList()
            }).ToList()
        };
    }
}
