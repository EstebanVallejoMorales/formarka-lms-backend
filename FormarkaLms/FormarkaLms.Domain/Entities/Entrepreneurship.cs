using FormarkaLms.Domain.Common;
using FormarkaLms.Domain.Enums;

namespace FormarkaLms.Domain.Entities;

public class Entrepreneurship : BaseEntity
{
    public string StudentId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Industry { get; set; }
    public EntrepreneurshipStage Stage { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? SocialMediaUrl { get; set; }
    public string? Location { get; set; }
    public string? LogoUrl { get; set; }

    // Navigation Properties
    public virtual Student Student { get; set; } = default!;
}
