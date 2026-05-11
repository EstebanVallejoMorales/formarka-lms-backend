using FormarkaLMS.Shared.Infrastructure.Enums;

namespace FormarkaLMS.Shared.Infrastructure.Models
{
    public class UserProfile
    {
        public Guid Id { get; set; } // Matches Supabase User ID
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
