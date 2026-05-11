using FormarkaLMS.Services.Learning.Domain.Interfaces;
using FormarkaLMS.Shared.Infrastructure.Models;
using FormarkaLMS.Shared.Infrastructure.Persistence;
using FormarkaLMS.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLMS.Services.Learning.Infrastructure.Repositories;

public class LessonProgressRepository : IRepository<LessonProgress>
{
    private readonly ApplicationDbContext _context;

    public LessonProgressRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LessonProgress?> GetByIdAsync(Guid id)
    {
        return await _context.LessonProgresses.FindAsync(id);
    }

    public async Task<IEnumerable<LessonProgress>> GetAllAsync()
    {
        return await _context.LessonProgresses.ToListAsync();
    }

    public async Task AddAsync(LessonProgress entity)
    {
        await _context.LessonProgresses.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(LessonProgress entity)
    {
        _context.LessonProgresses.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var progress = await _context.LessonProgresses.FindAsync(id);
        if (progress != null)
        {
            _context.LessonProgresses.Remove(progress);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<LessonProgress>> GetByStudentAndCourseAsync(Guid studentId, Guid courseId)
    {
        return await _context.LessonProgresses
            .Where(lp => lp.StudentId == studentId && lp.CourseId == courseId)
            .ToListAsync();
    }

    public async Task<LessonProgress?> GetByStudentAndLessonAsync(Guid studentId, Guid lessonId)
    {
        return await _context.LessonProgresses
            .FirstOrDefaultAsync(lp => lp.StudentId == studentId && lp.LessonId == lessonId);
    }

    public async Task<int> GetCompletedLessonsCountAsync(Guid studentId, Guid courseId)
    {
         return await _context.LessonProgresses
            .CountAsync(lp => lp.StudentId == studentId && lp.CourseId == courseId && lp.IsCompleted);
    }
}
