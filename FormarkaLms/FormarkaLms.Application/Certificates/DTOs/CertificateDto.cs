namespace FormarkaLms.Application.Certificates.DTOs;

public class CertificateDto
{
    public int Id { get; set; }
    public string CourseTitle { get; set; } = default!;
    public string StudentName { get; set; } = default!;
    public DateTime IssueDate { get; set; }
    public string CertificateCode { get; set; } = default!;
}
