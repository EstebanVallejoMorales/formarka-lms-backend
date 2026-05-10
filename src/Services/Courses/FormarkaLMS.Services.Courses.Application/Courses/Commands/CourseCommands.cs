using FormarkaLMS.Services.Courses.Application.DTOs;
using FormarkaLMS.Services.Courses.Domain.Interfaces;
using MediatR;

namespace FormarkaLMS.Services.Courses.Application.Courses.Commands;

public record UpdateCourseCommand(Guid Id, CreateCourseDto CourseDto) : IRequest<bool>;
public record DeleteCourseCommand(Guid Id) : IRequest<bool>;
public record PublishCourseCommand(Guid Id) : IRequest<bool>;
public record UnpublishCourseCommand(Guid Id) : IRequest<bool>;

public class CourseCommandHandlers : 
    IRequestHandler<UpdateCourseCommand, bool>,
    IRequestHandler<DeleteCourseCommand, bool>,
    IRequestHandler<PublishCourseCommand, bool>,
    IRequestHandler<UnpublishCourseCommand, bool>
{
    private readonly ICourseRepository _repository;

    public CourseCommandHandlers(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _repository.GetByIdAsync(request.Id);
        if (course == null) return false;

        course.Title = request.CourseDto.Title;
        course.Description = request.CourseDto.Description;
        course.CoverImageUrl = request.CourseDto.CoverImageUrl;
        course.Category = request.CourseDto.Category;
        course.Level = request.CourseDto.Level;
        course.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(course);
        return true;
    }

    public async Task<bool> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _repository.GetByIdAsync(request.Id);
        if (course == null) return false;

        await _repository.DeleteAsync(request.Id);
        return true;
    }

    public async Task<bool> Handle(PublishCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _repository.GetByIdAsync(request.Id);
        if (course == null) return false;

        course.IsPublished = true;
        course.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(course);
        return true;
    }

    public async Task<bool> Handle(UnpublishCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _repository.GetByIdAsync(request.Id);
        if (course == null) return false;

        course.IsPublished = false;
        course.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(course);
        return true;
    }
}
