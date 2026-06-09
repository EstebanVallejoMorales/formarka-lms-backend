using MediatR;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLms.Application.Users.Commands;

public record UpdateUserCommand(
    string Id, 
    string Name, 
    string Role, 
    string? Specialty
) : IRequest<bool>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Instructor)
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            
        if (user == null) return false;

        // Update basic info
        user.Name = request.Name;
        
        // Split name for First/Last if needed
        var names = request.Name.Split(' ', 2);
        user.FirstName = names[0];
        user.LastName = names.Length > 1 ? names[1] : string.Empty;

        // Update role if changed
        if (Enum.TryParse<UserRole>(request.Role, true, out var newRole))
        {
            if (user.Role != newRole)
            {
                // Handle role transition logic if necessary
                // For now just update the role
                user.Role = newRole;
                
                // If transitioning to Instructor, ensure record exists
                if (newRole == UserRole.Teacher && user.Instructor == null)
                {
                    _context.Instructors.Add(new FormarkaLms.Domain.Entities.Instructor { Id = user.Id });
                }
                // If transitioning to Student, ensure record exists
                if (newRole == UserRole.Student && user.Student == null)
                {
                    _context.Students.Add(new FormarkaLms.Domain.Entities.Student { Id = user.Id });
                }
            }
        }

        // Update specialty/title if instructor
        if (user.Instructor != null)
        {
            user.Instructor.ProfessionalTitle = request.Specialty;
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
