using FormarkaLMS.Services.Learning.Domain.Entities;
using FormarkaLMS.Shared.Interfaces;

namespace FormarkaLMS.Services.Learning.Domain.Interfaces;

public interface ILessonProgressRepository : IRepository<LessonProgress>
{
    Task<IEnumerable<LessonProgress>> GetByStudentAndCourseAsync(Guid studentId, Guid courseId);
    Task<LessonProgress?> GetByStudentAndLessonAsync(Guid studentId, Guid lessonId);
    Task<int> GetCompletedLessonsCountAsync(Guid studentId, Guid courseId);
}
