using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Domain.Entities;

namespace FormarkaLms.Application.Lessons.Commands;

public record CompleteLessonCommand(int LessonId, string UserId) : IRequest<bool>;

public class CompleteLessonCommandHandler : IRequestHandler<CompleteLessonCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public CompleteLessonCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CompleteLessonCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _context.Lessons.FindAsync(new object[] { request.LessonId }, cancellationToken);
        if (lesson == null) return false;

        var progress = await _context.LessonProgresses
            .FirstOrDefaultAsync(lp => lp.LessonId == request.LessonId && lp.StudentId == request.UserId, cancellationToken);

        if (progress == null)
        {
            progress = new LessonProgress
            {
                LessonId = request.LessonId,
                StudentId = request.UserId,
                IsCompleted = true,
                LastAccessed = DateTime.UtcNow
            };
            _context.LessonProgresses.Add(progress);
        }
        else
        {
            progress.IsCompleted = true;
            progress.LastAccessed = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
