namespace FormarkaLms.Application.Users.DTOs;

public class CompleteProfileDto
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // Admin, Teacher, Student
    public string Specialty { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
}
