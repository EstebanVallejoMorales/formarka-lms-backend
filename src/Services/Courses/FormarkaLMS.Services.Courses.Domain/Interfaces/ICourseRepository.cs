using FormarkaLMS.Services.Courses.Domain.Entities;
using FormarkaLMS.Shared.Interfaces;

namespace FormarkaLMS.Services.Courses.Domain.Interfaces;

public interface ICourseRepository : IRepository<Course>
{
    Task<IEnumerable<Course>> GetPublishedCoursesAsync();
    Task<Course?> GetCourseWithDetailsAsync(Guid id);
}
