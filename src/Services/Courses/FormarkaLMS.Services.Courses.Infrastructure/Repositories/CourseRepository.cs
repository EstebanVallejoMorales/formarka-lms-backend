using FormarkaLMS.Services.Courses.Domain.Entities;
using FormarkaLMS.Services.Courses.Domain.Interfaces;
using FormarkaLMS.Services.Courses.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLMS.Services.Courses.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly CoursesDbContext _context;

    public CourseRepository(CoursesDbContext context)
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
