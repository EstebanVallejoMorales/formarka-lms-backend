using FormarkaLMS.Services.Learning.Domain.Entities;
using FormarkaLMS.Shared.Interfaces;

namespace FormarkaLMS.Services.Learning.Domain.Interfaces;

public interface IQuizRepository : IRepository<Quiz>
{
    Task<Quiz?> GetByLessonIdAsync(Guid lessonId);
    Task<Quiz?> GetWithDetailsAsync(Guid id);
}
