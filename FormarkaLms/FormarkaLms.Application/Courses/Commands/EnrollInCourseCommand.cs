using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Domain.Entities;
using FormarkaLms.Domain.Enums;

namespace FormarkaLms.Application.Courses.Commands;

public record EnrollInCourseCommand(int CourseId, string UserId) : IRequest<bool>;

public class EnrollInCourseCommandHandler : IRequestHandler<EnrollInCourseCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public EnrollInCourseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(EnrollInCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses.FindAsync(new object[] { request.CourseId }, cancellationToken);
        if (course == null) return false;

        var existingEnrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.CourseId == request.CourseId && e.StudentId == request.UserId, cancellationToken);

        if (existingEnrollment != null) return true; // Already enrolled

        var enrollment = new Enrollment
        {
            CourseId = request.CourseId,
            StudentId = request.UserId,
            EnrollmentDate = DateTime.UtcNow,
            Status = EnrollmentStatus.Active,
            Progress = 0,
            IsCompleted = false
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
