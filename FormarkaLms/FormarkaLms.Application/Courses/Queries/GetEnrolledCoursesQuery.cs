using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;

namespace FormarkaLms.Application.Courses.Queries;

public record GetEnrolledCoursesQuery(string UserId) : IRequest<List<CourseDto>>;

public class GetEnrolledCoursesQueryHandler : IRequestHandler<GetEnrolledCoursesQuery, List<CourseDto>>
{
    private readonly IApplicationDbContext _context;

    public GetEnrolledCoursesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CourseDto>> Handle(GetEnrolledCoursesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Enrollments
            .Where(e => e.StudentId == request.UserId)
            .Include(e => e.Course)
            .ThenInclude(c => c.Instructor)
            .ThenInclude(i => i.User)
            .Select(e => new CourseDto
            {
                Id = e.Course.Id,
                Title = e.Course.Title,
                Description = e.Course.Description,
                ThumbnailUrl = e.Course.ThumbnailUrl,
                Category = e.Course.Category,
                Level = e.Course.Level.ToString().ToLower(),
                InstructorId = e.Course.InstructorId,
                InstructorName = e.Course.Instructor.User.Name,
                TotalHours = e.Course.TotalHours
            })
            .ToListAsync(cancellationToken);
    }
}
