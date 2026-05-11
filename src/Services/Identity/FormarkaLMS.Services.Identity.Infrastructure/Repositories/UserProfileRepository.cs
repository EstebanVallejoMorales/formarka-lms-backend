using FormarkaLMS.Shared.Infrastructure.Models;
using FormarkaLMS.Shared.Infrastructure.Persistence;
using FormarkaLMS.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLMS.Services.Identity.Infrastructure.Repositories;

public class UserProfileRepository : IRepository<UserProfile>
{
    private readonly ApplicationDbContext _context;

    public UserProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfile?> GetByIdAsync(Guid id)
    {
        return await _context.UserProfiles.FindAsync(id);
    }

    public async Task<IEnumerable<UserProfile>> GetAllAsync()
    {
        return await _context.UserProfiles.ToListAsync();
    }

    public async Task AddAsync(UserProfile entity)
    {
        await _context.UserProfiles.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserProfile entity)
    {
        _context.UserProfiles.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.UserProfiles.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<UserProfile?> GetByEmailAsync(string email)
    {
        return await _context.UserProfiles.FirstOrDefaultAsync(u => u.Email == email);
    }
}
