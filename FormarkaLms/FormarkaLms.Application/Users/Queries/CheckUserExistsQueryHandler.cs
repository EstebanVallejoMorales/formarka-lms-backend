using MediatR;
using FormarkaLms.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLms.Application.Users.Queries;

public class CheckUserExistsQueryHandler : IRequestHandler<CheckUserExistsQuery, bool>
{
    private readonly IApplicationDbContext _context;

    public CheckUserExistsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CheckUserExistsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(u => u.Id == request.Id, cancellationToken);
    }
}
