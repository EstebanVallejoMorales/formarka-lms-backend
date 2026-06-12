using MediatR;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLms.Application.Users.Commands;

public record UpdateUserRoleCommand(string UserId, string Role) : IRequest<bool>;

public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateUserRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Instructor)
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
            
        if (user == null) return false;

        if (Enum.TryParse<UserRole>(request.Role, true, out var newRole))
        {
            if (user.Role != newRole)
            {
                user.Role = newRole;

                // Ensure Instructor record if role is Teacher
                if (newRole == UserRole.Teacher && user.Instructor == null)
                {
                    _context.Instructors.Add(new FormarkaLms.Domain.Entities.Instructor { Id = user.Id });
                }
                
                // Ensure Student record if role is Student
                if (newRole == UserRole.Student && user.Student == null)
                {
                    _context.Students.Add(new FormarkaLms.Domain.Entities.Student { Id = user.Id });
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }
}
