using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;

namespace FormarkaLms.Application.Courses.Queries;

public record GetEnrolledStudentsQuery(int CourseId) : IRequest<List<StudentProgressDto>>;

public class StudentProgressDto
{
    public string StudentId { get; set; } = default!;
    public string StudentName { get; set; } = default!;
    public int Progress { get; set; }
    public decimal? Grade { get; set; }
    public string? CompletedDate { get; set; }
}

public class GetEnrolledStudentsQueryHandler : IRequestHandler<GetEnrolledStudentsQuery, List<StudentProgressDto>>
{
    private readonly IApplicationDbContext _context;

    public GetEnrolledStudentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudentProgressDto>> Handle(GetEnrolledStudentsQuery request, CancellationToken cancellationToken)
    {
        var totalLessons = await _context.Lessons
            .Where(l => l.Module.CourseId == request.CourseId)
            .CountAsync(cancellationToken);

        var enrollments = await _context.Enrollments
            .Include(e => e.Student.User)
            .Where(e => e.CourseId == request.CourseId)
            .ToListAsync(cancellationToken);

        var result = new List<StudentProgressDto>();

        foreach (var enrollment in enrollments)
        {
            var completedLessons = await _context.LessonProgresses
                .Where(p => p.UserId == enrollment.StudentId && p.Lesson.Module.CourseId == request.CourseId && p.IsCompleted)
                .CountAsync(cancellationToken);

            int progress = totalLessons == 0 ? 0 : (completedLessons * 100) / totalLessons;

            result.Add(new StudentProgressDto
            {
                StudentId = enrollment.StudentId,
                StudentName = enrollment.Student.User.FullName,
                Progress = progress,
                // Simple average grade for now
                Grade = null, 
                CompletedDate = progress == 100 ? enrollment.EnrolledAt.AddDays(7).ToString("yyyy-MM-dd") : null
            });
        }

        return result;
    }
}
