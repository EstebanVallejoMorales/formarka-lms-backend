using MediatR;
using FormarkaLms.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLms.Application.Users.Commands;

public record DeleteUserCommand(string UserId, string CurrentAdminId) : IRequest<bool>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISupabaseService _supabaseService;

    public DeleteUserCommandHandler(IApplicationDbContext context, ISupabaseService supabaseService)
    {
        _context = context;
        _supabaseService = supabaseService;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Prevent self-deletion
        if (request.UserId == request.CurrentAdminId)
        {
            throw new InvalidOperationException("Un administrador no puede eliminarse a sí mismo.");
        }

        var user = await _context.Users
            .Include(u => u.Student)
            .Include(u => u.Instructor)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
            
        if (user == null) return false;

        // 2. Delete from Supabase Auth first
        await _supabaseService.DeleteUserAsync(request.UserId);

        // 3. Clean up related data manually to avoid FK violations
        
        // General user references
        var certificates = await _context.Certificates.Where(c => c.UserId == request.UserId).ToListAsync(cancellationToken);
        _context.Certificates.RemoveRange(certificates);

        var comments = await _context.Comments.Where(c => c.UserId == request.UserId).ToListAsync(cancellationToken);
        _context.Comments.RemoveRange(comments);

        // Student related data
        if (user.Student != null)
        {
            var enrollments = await _context.Enrollments.Where(e => e.StudentId == request.UserId).ToListAsync(cancellationToken);
            _context.Enrollments.RemoveRange(enrollments);

            var deliverables = await _context.Deliverables.Where(d => d.StudentId == request.UserId).ToListAsync(cancellationToken);
            _context.Deliverables.RemoveRange(deliverables);

            var attempts = await _context.QuizAttempts.Where(a => a.StudentId == request.UserId).ToListAsync(cancellationToken);
            _context.QuizAttempts.RemoveRange(attempts);

            var progress = await _context.LessonProgresses.Where(p => p.StudentId == request.UserId).ToListAsync(cancellationToken);
            _context.LessonProgresses.RemoveRange(progress);

            var entrepreneurships = await _context.Entrepreneurships.Where(e => e.StudentId == request.UserId).ToListAsync(cancellationToken);
            _context.Entrepreneurships.RemoveRange(entrepreneurships);

            _context.Students.Remove(user.Student);
        }

        // Instructor related data
        if (user.Instructor != null)
        {
            // When an instructor is deleted, we keep their courses but set InstructorId to null.
            var courses = await _context.Courses.Where(c => c.InstructorId == request.UserId).ToListAsync(cancellationToken);
            foreach (var course in courses)
            {
                course.InstructorId = null;
            }
            _context.Instructors.Remove(user.Instructor);
        }

        // 4. Finally, remove the User
        _context.Users.Remove(user);
        
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
