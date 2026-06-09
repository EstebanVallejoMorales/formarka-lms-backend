using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;

namespace FormarkaLms.Application.Courses.Queries;

public class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, List<CourseDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCoursesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CourseDto>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Courses
            .Include(c => c.Instructor)
            .ThenInclude(i => i.User)
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                ThumbnailUrl = c.ThumbnailUrl,
                Category = c.Category,
                Level = c.Level.ToString().ToLower(),
                InstructorId = c.InstructorId,
                InstructorName = c.Instructor.User.Name,
                TotalHours = c.TotalHours
            })
            .ToListAsync(cancellationToken);
    }
}
