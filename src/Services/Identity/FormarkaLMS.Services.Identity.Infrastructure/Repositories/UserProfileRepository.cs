using FormarkaLMS.Services.Identity.Domain.Entities;
using FormarkaLMS.Services.Identity.Domain.Interfaces;
using FormarkaLMS.Services.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLMS.Services.Identity.Infrastructure.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly IdentityDbContext _context;

    public UserProfileRepository(IdentityDbContext context)
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
