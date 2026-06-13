using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;

namespace FormarkaLms.Application.Courses.Commands;

public record UnenrollStudentCommand(int CourseId, string StudentId) : IRequest<bool>;

public class UnenrollStudentCommandHandler : IRequestHandler<UnenrollStudentCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UnenrollStudentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UnenrollStudentCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.CourseId == request.CourseId && e.StudentId == request.StudentId, cancellationToken);

        if (enrollment == null) return false;

        _context.Enrollments.Remove(enrollment);
        
        // Also remove lesson progress for this student in this course
        var progress = await _context.LessonProgresses
            .Where(lp => lp.StudentId == request.StudentId && lp.Lesson.Module.CourseId == request.CourseId)
            .ToListAsync(cancellationToken);
            
        _context.LessonProgresses.RemoveRange(progress);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
