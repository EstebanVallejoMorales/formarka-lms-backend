using FormarkaLMS.Services.Learning.Domain.Entities;
using FormarkaLMS.Shared.Interfaces;

namespace FormarkaLMS.Services.Learning.Domain.Interfaces;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<IEnumerable<Enrollment>> GetByStudentIdAsync(Guid studentId);
    Task<Enrollment?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId);
}
