using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Application.Certificates.DTOs;
using FormarkaLms.Domain.Entities;

namespace FormarkaLms.Application.Certificates.Queries;

public record GetCertificateQuery(int CourseId, string UserId) : IRequest<CertificateDto?>;

public class GetCertificateQueryHandler : IRequestHandler<GetCertificateQuery, CertificateDto?>
{
    private readonly IApplicationDbContext _context;

    public GetCertificateQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CertificateDto?> Handle(GetCertificateQuery request, CancellationToken cancellationToken)
    {
        // 1. Check if course is completed
        var totalLessons = await _context.Lessons
            .Where(l => l.Module.CourseId == request.CourseId)
            .CountAsync(cancellationToken);

        var completedLessons = await _context.LessonProgresses
            .Where(p => p.UserId == request.UserId && p.Lesson.Module.CourseId == request.CourseId && p.IsCompleted)
            .CountAsync(cancellationToken);

        if (totalLessons == 0 || completedLessons < totalLessons)
        {
            return null;
        }

        // 2. Check if certificate already exists
        var certificate = await _context.Certificates
            .Include(c => c.Course)
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.CourseId == request.CourseId && c.UserId == request.UserId, cancellationToken);

        if (certificate == null)
        {
            // 3. Create new certificate if not exists
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
