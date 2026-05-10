using FormarkaLMS.Services.Courses.Application.DTOs;
using FormarkaLMS.Services.Courses.Domain.Entities;
using FormarkaLMS.Services.Courses.Domain.Interfaces;
using MediatR;

namespace FormarkaLMS.Services.Courses.Application.Courses.Commands;

public record CreateCourseCommand(CreateCourseDto CourseDto) : IRequest<Guid>;

public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, Guid>
{
    private readonly ICourseRepository _repository;

    public CreateCourseHandler(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = request.CourseDto.Title,
            Description = request.CourseDto.Description,
            CoverImageUrl = request.CourseDto.CoverImageUrl,
            Category = request.CourseDto.Category,
            Level = request.CourseDto.Level,
            IsPublished = false,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(course);
        return course.Id;
    }
}
