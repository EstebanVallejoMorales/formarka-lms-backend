using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Application.Deliverables.Common;
using FormarkaLms.Domain.Entities;

namespace FormarkaLms.Application.Deliverables.Handlers;

public class DeliverableHandlers : 
    IRequestHandler<SubmitDeliverableCommand, DeliverableDto>,
    IRequestHandler<GradeDeliverableCommand, bool>,
    IRequestHandler<GetDeliverableByLessonQuery, DeliverableDto?>,
    IRequestHandler<GetStudentDeliverablesInCourseQuery, List<DeliverableDto>>
{
    private readonly IApplicationDbContext _context;

    public DeliverableHandlers(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DeliverableDto>> Handle(GetStudentDeliverablesInCourseQuery request, CancellationToken cancellationToken)
    {
        var deliverables = await _context.Deliverables
            .Include(d => d.Student.User)
            .Where(d => d.StudentId == request.StudentId && d.Lesson.Module.CourseId == request.CourseId)
            .ToListAsync(cancellationToken);

        return deliverables.Select(d => new DeliverableDto
        {
            Id = d.Id,
            StudentId = d.StudentId,
            StudentName = d.Student.User.FullName,
            LessonId = d.LessonId,
            ContentUrl = d.ContentUrl,
            SubmissionDate = d.SubmissionDate,
            Grade = d.Grade,
            Feedback = d.Feedback,
            Status = d.Status.ToString().ToLower()
        }).ToList();
    }

    public async Task<DeliverableDto> Handle(SubmitDeliverableCommand request, CancellationToken cancellationToken)
    {
        var deliverable = await _context.Deliverables
            .FirstOrDefaultAsync(d => d.LessonId == request.LessonId && d.StudentId == request.UserId, cancellationToken);

        if (deliverable == null)
        {
            deliverable = new Deliverable
            {
                LessonId = request.LessonId,
                StudentId = request.UserId,
                ContentUrl = request.ContentUrl,
                SubmissionDate = DateTime.UtcNow,
                Status = FormarkaLms.Domain.Enums.DeliverableStatus.Pending
            };
            _context.Deliverables.Add(deliverable);
        }
        else
        {
            deliverable.ContentUrl = request.ContentUrl;
            deliverable.SubmissionDate = DateTime.UtcNow;
            deliverable.Status = FormarkaLms.Domain.Enums.DeliverableStatus.Pending; // Reset to pending if resubmitted
        }

        await _context.SaveChangesAsync(cancellationToken);

        var student = await _context.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == request.UserId, cancellationToken);

        return new DeliverableDto
        {
            Id = deliverable.Id,
            StudentId = deliverable.StudentId,
            StudentName = student?.User.Name ?? "Student",
            LessonId = deliverable.LessonId,
            ContentUrl = deliverable.ContentUrl,
            SubmissionDate = deliverable.SubmissionDate,
            Status = deliverable.Status.ToString().ToLower()
        };
    }

    public async Task<bool> Handle(GradeDeliverableCommand request, CancellationToken cancellationToken)
    {
        var deliverable = await _context.Deliverables.FindAsync(new object[] { request.DeliverableId }, cancellationToken);
        if (deliverable == null) return false;

        deliverable.Grade = request.Grade;
        deliverable.Feedback = request.Feedback;
        deliverable.Status = FormarkaLms.Domain.Enums.DeliverableStatus.Graded;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<DeliverableDto?> Handle(GetDeliverableByLessonQuery request, CancellationToken cancellationToken)
    {
        var deliverable = await _context.Deliverables
            .Include(d => d.Student)
            .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(d => d.LessonId == request.LessonId && d.StudentId == request.UserId, cancellationToken);

        if (deliverable == null) return null;

        return new DeliverableDto
        {
            Id = deliverable.Id,
            StudentId = deliverable.StudentId,
            StudentName = deliverable.Student.User.Name,
            LessonId = deliverable.LessonId,
            ContentUrl = deliverable.ContentUrl,
            SubmissionDate = deliverable.SubmissionDate,
            Grade = deliverable.Grade,
            Feedback = deliverable.Feedback,
            Status = deliverable.Status.ToString().ToLower()
        };
    }
}
