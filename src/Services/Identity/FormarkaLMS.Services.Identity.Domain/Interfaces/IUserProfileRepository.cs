using FormarkaLMS.Services.Identity.Domain.Entities;
using FormarkaLMS.Shared.Interfaces;

namespace FormarkaLMS.Services.Identity.Domain.Interfaces;

public interface IUserProfileRepository : IRepository<UserProfile>
{
    Task<UserProfile?> GetByEmailAsync(string email);
}
