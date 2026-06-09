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
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user == null) return false;

        if (Enum.TryParse<UserRole>(request.Role, true, out var newRole))
        {
            user.Role = newRole;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }
}
