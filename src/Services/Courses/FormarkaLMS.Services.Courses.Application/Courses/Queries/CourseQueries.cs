using FormarkaLMS.Services.Courses.Application.DTOs;
using FormarkaLMS.Services.Courses.Domain.Interfaces;
using MediatR;

namespace FormarkaLMS.Services.Courses.Application.Courses.Queries;

public record GetAllCoursesQuery() : IRequest<IEnumerable<CourseDto>>;
public record GetPublishedCoursesQuery() : IRequest<IEnumerable<CourseDto>>;
public record GetCourseByIdQuery(Guid Id) : IRequest<CourseDto?>;

public class CourseQueryHandlers : 
    IRequestHandler<GetAllCoursesQuery, IEnumerable<CourseDto>>,
    IRequestHandler<GetPublishedCoursesQuery, IEnumerable<CourseDto>>,
    IRequestHandler<GetCourseByIdQuery, CourseDto?>
{
    private readonly ICourseRepository _repository;

    public CourseQueryHandlers(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CourseDto>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _repository.GetAllAsync();
        return courses.Select(c => new CourseDto(c.Id, c.Title, c.Description, c.CoverImageUrl, c.Category, c.Level, c.IsPublished));
    }

    public async Task<IEnumerable<CourseDto>> Handle(GetPublishedCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _repository.GetPublishedCoursesAsync();
        return courses.Select(c => new CourseDto(c.Id, c.Title, c.Description, c.CoverImageUrl, c.Category, c.Level, c.IsPublished));
    }

    public async Task<CourseDto?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _repository.GetByIdAsync(request.Id);
        if (course == null) return null;

        return new CourseDto(course.Id, course.Title, course.Description, course.CoverImageUrl, course.Category, course.Level, course.IsPublished);
    }
}
