using FormarkaLMS.Services.Learning.Domain.Entities;
using FormarkaLMS.Shared.Interfaces;

namespace FormarkaLMS.Services.Learning.Domain.Interfaces;

public interface IQuizResultRepository : IRepository<QuizResult>
{
    Task<IEnumerable<QuizResult>> GetByStudentAndQuizAsync(Guid studentId, Guid quizId);
}
