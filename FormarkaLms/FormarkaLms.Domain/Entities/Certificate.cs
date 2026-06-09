using FormarkaLms.Domain.Common;

namespace FormarkaLms.Domain.Entities;

public class Certificate : BaseEntity
{
    public int CourseId { get; set; }
    public string UserId { get; set; } = default!;
    public DateTime IssueDate { get; set; }
    public string CertificateCode { get; set; } = default!;

    // Navigation Properties
    public virtual Course Course { get; set; } = default!;
    public virtual User User { get; set; } = default!;
}
