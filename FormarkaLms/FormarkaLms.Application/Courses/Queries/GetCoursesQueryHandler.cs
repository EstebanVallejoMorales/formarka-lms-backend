using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Domain.Enums;

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
        var query = _context.Courses.AsQueryable();

        if (!string.IsNullOrEmpty(request.InstructorId))
        {
            query = query.Where(c => c.InstructorId == request.InstructorId);
        }

        return await query
            .Include(c => c.Instructor)
            .ThenInclude(i => i.User)
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                ThumbnailUrl = c.ThumbnailUrl,
                Category = c.Category,
                Level = c.Level == CourseLevel.Basico ? "básico" :
                        c.Level == CourseLevel.Intermedio ? "intermedio" :
                        c.Level == CourseLevel.Avanzado ? "avanzado" : "básico",
                InstructorId = c.InstructorId,
                InstructorName = c.Instructor.User.Name,
                TotalHours = c.TotalHours
            })
            .ToListAsync(cancellationToken);
    }
}
