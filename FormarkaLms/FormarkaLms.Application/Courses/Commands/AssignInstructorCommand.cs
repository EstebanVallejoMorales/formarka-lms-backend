using MediatR;
using FormarkaLms.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLms.Application.Courses.Commands;

public record AssignInstructorCommand(int CourseId, string InstructorId) : IRequest<bool>;

public class AssignInstructorCommandHandler : IRequestHandler<AssignInstructorCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public AssignInstructorCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(AssignInstructorCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);
        if (course == null) return false;

        // Verify if instructor exists
        var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Id == request.InstructorId, cancellationToken);
        if (instructor == null)
        {
            // If not in Instructor table but is a User with Teacher role, we should probably add them to Instructor table
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.InstructorId, cancellationToken);
            if (user == null || user.Role != FormarkaLms.Domain.Enums.UserRole.Teacher) return false;

            instructor = new FormarkaLms.Domain.Entities.Instructor { Id = user.Id };
            _context.Instructors.Add(instructor);
        }

        course.InstructorId = request.InstructorId;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
