using FormarkaLMS.Services.Learning.Domain.Entities;
using FormarkaLMS.Services.Learning.Domain.Interfaces;
using MediatR;

namespace FormarkaLMS.Services.Learning.Application.Progress.Commands;

public record MarkLessonAsCompletedCommand(Guid StudentId, Guid CourseId, Guid LessonId) : IRequest<bool>;

public class MarkLessonAsCompletedHandler : IRequestHandler<MarkLessonAsCompletedCommand, bool>
{
    private readonly ILessonProgressRepository _progressRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public MarkLessonAsCompletedHandler(ILessonProgressRepository progressRepository, IEnrollmentRepository enrollmentRepository)
    {
        _progressRepository = progressRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<bool> Handle(MarkLessonAsCompletedCommand request, CancellationToken cancellationToken)
    {
        var progress = await _progressRepository.GetByStudentAndLessonAsync(request.StudentId, request.LessonId);
        
        if (progress == null)
        {
            progress = new LessonProgress
            {
                Id = Guid.NewGuid(),
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                LessonId = request.LessonId,
                IsCompleted = true,
                CompletedAt = DateTime.UtcNow
            };
            await _progressRepository.AddAsync(progress);
        }
        else if (!progress.IsCompleted)
        {
            progress.IsCompleted = true;
            progress.CompletedAt = DateTime.UtcNow;
            await _progressRepository.UpdateAsync(progress);
        }

        return true;
    }
}
