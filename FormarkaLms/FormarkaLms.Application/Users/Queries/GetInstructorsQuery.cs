using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Domain.Enums;

namespace FormarkaLms.Application.Users.Queries;

public record InstructorDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Specialty { get; set; }
}

public record GetInstructorsQuery : IRequest<List<InstructorDto>>;

public class GetInstructorsQueryHandler : IRequestHandler<GetInstructorsQuery, List<InstructorDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInstructorsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<InstructorDto>> Handle(GetInstructorsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Users
            .Where(u => u.Role == UserRole.Teacher)
            .Select(u => new InstructorDto
            {
                Id = u.Id,
                Name = u.Name,
                Specialty = u.Specialty
            })
            .ToListAsync(cancellationToken);
    }
}
