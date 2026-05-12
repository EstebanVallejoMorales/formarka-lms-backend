
using FormarkaLMS.Shared.Infrastructure.Enums;

namespace FormarkaLMS.Services.Courses.Application.DTOs;

public record CourseDto(
    Guid Id,
    string Title,
    string Description,
    string CoverImageUrl,
    string Category,
    CourseLevel Level,
    bool IsPublished
);

public record CreateCourseDto(
    string Title,
    string Description,
    string CoverImageUrl,
    string Category,
    CourseLevel Level
);
