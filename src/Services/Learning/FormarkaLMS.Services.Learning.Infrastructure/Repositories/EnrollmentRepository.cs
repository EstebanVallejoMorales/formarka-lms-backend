using FormarkaLMS.Services.Learning.Domain.Entities;
using FormarkaLMS.Services.Learning.Domain.Interfaces;
using FormarkaLMS.Services.Learning.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLMS.Services.Learning.Infrastructure.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly LearningDbContext _context;

    public EnrollmentRepository(LearningDbContext context)
    {
        _context = context;
    }

    public async Task<Enrollment?> GetByIdAsync(Guid id)
    {
        return await _context.Enrollments.FindAsync(id);
    }

    public async Task<IEnumerable<Enrollment>> GetAllAsync()
    {
        return await _context.Enrollments.ToListAsync();
    }

    public async Task AddAsync(Enrollment entity)
    {
        await _context.Enrollments.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Enrollment entity)
    {
        _context.Enrollments.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment != null)
        {
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Enrollments
            .Where(e => e.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<Enrollment?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId)
    {
        return await _context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
    }
}
