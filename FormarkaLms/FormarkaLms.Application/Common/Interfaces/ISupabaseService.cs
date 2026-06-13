namespace FormarkaLms.Application.Common.Interfaces;

public interface ISupabaseService
{
    Task<bool> DeleteUserAsync(string userId);
}
