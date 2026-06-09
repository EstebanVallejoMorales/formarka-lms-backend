using MediatR;
using FormarkaLms.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLms.Application.Courses.Commands;

public record DeleteCourseCommand(int Id) : IRequest<bool>;

public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteCourseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses.FindAsync(new object[] { request.Id }, cancellationToken);
        if (course == null) return false;

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
