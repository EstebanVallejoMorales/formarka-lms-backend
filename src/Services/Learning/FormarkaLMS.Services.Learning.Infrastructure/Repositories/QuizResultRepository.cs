using FormarkaLMS.Services.Learning.Domain.Interfaces;
using FormarkaLMS.Shared.Infrastructure.Models;
using FormarkaLMS.Shared.Infrastructure.Persistence;
using FormarkaLMS.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLMS.Services.Learning.Infrastructure.Repositories;

public class QuizResultRepository : IRepository<QuizResult>
{
    private readonly ApplicationDbContext _context;

    public QuizResultRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<QuizResult?> GetByIdAsync(Guid id)
    {
        return await _context.QuizResults.FindAsync(id);
    }

    public async Task<IEnumerable<QuizResult>> GetAllAsync()
    {
        return await _context.QuizResults.ToListAsync();
    }

    public async Task AddAsync(QuizResult entity)
    {
        await _context.QuizResults.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(QuizResult entity)
    {
        _context.QuizResults.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var result = await _context.QuizResults.FindAsync(id);
        if (result != null)
        {
            _context.QuizResults.Remove(result);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<QuizResult>> GetByStudentAndQuizAsync(Guid studentId, Guid quizId)
    {
        return await _context.QuizResults
            .Where(qr => qr.StudentId == studentId && qr.QuizId == quizId)
            .OrderByDescending(qr => qr.TakenAt)
            .ToListAsync();
    }
}
