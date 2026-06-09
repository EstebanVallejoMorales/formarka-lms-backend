using MediatR;
using FormarkaLms.Domain.Enums;

namespace FormarkaLms.Application.Deliverables.Common;

public class DeliverableDto
{
    public int Id { get; set; }
    public string StudentId { get; set; } = default!;
    public string StudentName { get; set; } = default!;
    public int LessonId { get; set; }
    public string ContentUrl { get; set; } = default!;
    public DateTime SubmissionDate { get; set; }
    public decimal? Grade { get; set; }
    public string? Feedback { get; set; }
    public string Status { get; set; } = default!;
}

public record SubmitDeliverableCommand(int LessonId, string UserId, string ContentUrl) : IRequest<DeliverableDto>;

public record GradeDeliverableCommand(int DeliverableId, decimal Grade, string? Feedback) : IRequest<bool>;

public record GetDeliverableByLessonQuery(int LessonId, string UserId) : IRequest<DeliverableDto?>;

public record GetStudentDeliverablesInCourseQuery(int CourseId, string StudentId) : IRequest<List<DeliverableDto>>;
