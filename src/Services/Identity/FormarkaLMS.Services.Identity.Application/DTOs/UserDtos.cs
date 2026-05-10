using FormarkaLMS.Services.Identity.Domain.Enums;

namespace FormarkaLMS.Services.Identity.Application.DTOs;

public record UserProfileDto(
    Guid Id,
    string Email,
    string FullName,
    string? AvatarUrl,
    UserRole Role
);

public record UpdateUserProfileDto(
    string FullName,
    string? AvatarUrl
);
