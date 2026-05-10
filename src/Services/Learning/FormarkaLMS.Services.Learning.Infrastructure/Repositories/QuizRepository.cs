using FormarkaLMS.Services.Learning.Domain.Entities;
using FormarkaLMS.Services.Learning.Domain.Interfaces;
using FormarkaLMS.Services.Learning.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLMS.Services.Learning.Infrastructure.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly LearningDbContext _context;

    public QuizRepository(LearningDbContext context)
    {
        _context = context;
    }

    public async Task<Quiz?> GetByIdAsync(Guid id)
    {
        return await _context.Quizzes.FindAsync(id);
    }

    public async Task<IEnumerable<Quiz>> GetAllAsync()
    {
        return await _context.Quizzes.ToListAsync();
    }

    public async Task AddAsync(Quiz entity)
    {
        await _context.Quizzes.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Quiz entity)
    {
        _context.Quizzes.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var quiz = await _context.Quizzes.FindAsync(id);
        if (quiz != null)
        {
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Quiz?> GetByLessonIdAsync(Guid lessonId)
    {
        return await _context.Quizzes
            .FirstOrDefaultAsync(q => q.LessonId == lessonId);
    }

    public async Task<Quiz?> GetWithDetailsAsync(Guid id)
    {
        return await _context.Quizzes
            .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == id);
    }
}
