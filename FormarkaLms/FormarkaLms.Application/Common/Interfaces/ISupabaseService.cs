namespace FormarkaLms.Application.Common.Interfaces;

public interface ISupabaseService
{
    Task<bool> DeleteUserAsync(string userId);
    Task<bool> UpdateUserPasswordAsync(string userId, string newPassword);
}
