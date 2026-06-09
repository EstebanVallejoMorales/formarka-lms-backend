using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;

namespace FormarkaLms.Application.Users.Queries;

public record GetAllUsersQuery : IRequest<List<UserAdminDto>>;

public class UserAdminDto
{
    public string Id { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string? Specialty { get; set; }
    public int EnrolledCoursesCount { get; set; }
}

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserAdminDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserAdminDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .Include(u => u.Student)
            .Include(u => u.Instructor)
            .Select(u => new UserAdminDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role.ToString(),
                Specialty = u.Instructor != null ? u.Instructor.ProfessionalTitle : null,
                EnrolledCoursesCount = _context.Enrollments.Count(e => e.StudentId == u.Id)
            })
            .ToListAsync(cancellationToken);

        return users;
    }
}
