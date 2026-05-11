using FormarkaLMS.Services.Courses.Domain.Interfaces;
using FormarkaLMS.Shared.Infrastructure.Models;
using FormarkaLMS.Shared.Infrastructure.Persistence;
using FormarkaLMS.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLMS.Services.Courses.Infrastructure.Repositories;

public class CourseRepository : IRepository<Course>
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        return await _context.Courses.FindAsync(id);
    }

    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        return await _context.Courses.ToListAsync();
    }

    public async Task AddAsync(Course entity)
    {
        await _context.Courses.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course entity)
    {
        _context.Courses.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course != null)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Course>> GetPublishedCoursesAsync()
    {
        return await _context.Courses
            .Where(c => c.IsPublished)
            .ToListAsync();
    }

    public async Task<Course?> GetCourseWithDetailsAsync(Guid id)
    {
        return await _context.Courses
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}
