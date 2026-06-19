using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Application.Certificates.DTOs;
using FormarkaLms.Domain.Entities;
using FormarkaLms.Domain.Enums;

namespace FormarkaLms.Application.Certificates.Queries;

public record GetCertificateQuery(int CourseId, string UserId) : IRequest<CertificateDto?>;

/// <summary>
/// Reason why a certificate cannot be issued yet.
/// </summary>
public enum CertificateBlockReason
{
    None,
    CourseNotCompleted,
    PendingDeliverables,   // Student has deliverable lessons without a submission
    FailedDeliverables     // Student has submissions that were graded below the passing threshold
}

/// <summary>
/// Wraps the certificate result with a reason when it cannot be issued.
/// </summary>
public class CertificateResult
{
    public CertificateDto? Certificate { get; init; }
    public CertificateBlockReason BlockReason { get; init; } = CertificateBlockReason.None;
    public bool IsEligible => Certificate != null;
}

public class GetCertificateQueryHandler : IRequestHandler<GetCertificateQuery, CertificateDto?>
{
    private readonly IApplicationDbContext _context;
    private const decimal PassingGrade = 60m;

    public GetCertificateQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CertificateDto?> Handle(GetCertificateQuery request, CancellationToken cancellationToken)
    {
        // 1. Verify 100% lesson completion
        var totalLessons = await _context.Lessons
            .Where(l => l.Module.CourseId == request.CourseId)
            .CountAsync(cancellationToken);

        var completedLessons = await _context.LessonProgresses
            .Where(p => p.StudentId == request.UserId
                     && p.Lesson.Module.CourseId == request.CourseId
                     && p.IsCompleted)
            .CountAsync(cancellationToken);

        if (totalLessons == 0 || completedLessons < totalLessons)
        {
            return null;
        }

        // 2. Verify all deliverable-type lessons have an approved submission
        var deliverableLessonIds = await _context.Lessons
            .Where(l => l.Module.CourseId == request.CourseId && l.Type == LessonType.Deliverable)
            .Select(l => l.Id)
            .ToListAsync(cancellationToken);

        if (deliverableLessonIds.Count > 0)
        {
            var studentDeliverables = await _context.Deliverables
                .Where(d => d.StudentId == request.UserId && deliverableLessonIds.Contains(d.LessonId))
                .ToListAsync(cancellationToken);

            // Every deliverable lesson must have a submission
            var submittedLessonIds = studentDeliverables.Select(d => d.LessonId).Distinct().ToHashSet();
            bool hasMissingSubmissions = deliverableLessonIds.Any(id => !submittedLessonIds.Contains(id));
            if (hasMissingSubmissions)
            {
                return null; // pending deliverable submissions
            }

            // Every submission must be graded with a passing grade
            bool hasFailedOrUngradedDeliverable = studentDeliverables.Any(
                d => d.Status != DeliverableStatus.Graded || (d.Grade.HasValue && d.Grade.Value < PassingGrade));
            if (hasFailedOrUngradedDeliverable)
            {
                return null; // deliverables not yet reviewed or failing grade
            }
        }

        // 3. Check if certificate already exists or create a new one
        var certificate = await _context.Certificates
            .Include(c => c.Course)
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.CourseId == request.CourseId && c.UserId == request.UserId, cancellationToken);

        if (certificate == null)
        {
            certificate = new Certificate
            {
                CourseId = request.CourseId,
                UserId = request.UserId,
                IssueDate = DateTime.UtcNow,
                CertificateCode = $"CERT-{request.CourseId}-{request.UserId.Substring(0, 8)}-{DateTime.UtcNow.Ticks % 10000}"
            };

            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync(cancellationToken);

            // Reload to get navigation properties
            certificate = await _context.Certificates
                .Include(c => c.Course)
                .Include(c => c.User)
                .FirstAsync(c => c.Id == certificate.Id, cancellationToken);
        }

        return new CertificateDto
        {
            Id = certificate.Id,
            CourseTitle = certificate.Course.Title,
            StudentName = certificate.User.FullName,
            IssueDate = certificate.IssueDate,
            CertificateCode = certificate.CertificateCode
        };
    }
}
