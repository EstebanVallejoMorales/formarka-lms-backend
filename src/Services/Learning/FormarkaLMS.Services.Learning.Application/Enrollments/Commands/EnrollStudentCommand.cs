using FormarkaLMS.Services.Learning.Domain.Entities;
using FormarkaLMS.Services.Learning.Domain.Interfaces;
using MediatR;

namespace FormarkaLMS.Services.Learning.Application.Enrollments.Commands;

public record EnrollStudentCommand(Guid StudentId, Guid CourseId) : IRequest<Guid>;

public class EnrollStudentHandler : IRequestHandler<EnrollStudentCommand, Guid>
{
    private readonly IEnrollmentRepository _repository;

    public EnrollStudentHandler(IEnrollmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
    {
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrolledAt = DateTime.UtcNow,
            IsCompleted = false,
            ProgressPercentage = 0
        };

        await _repository.AddAsync(enrollment);
        return enrollment.Id;
    }
}
