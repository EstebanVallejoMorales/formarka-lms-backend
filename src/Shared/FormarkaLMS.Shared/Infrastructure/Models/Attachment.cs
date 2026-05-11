namespace FormarkaLMS.Shared.Infrastructure.Models
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public Guid LessonId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty; // PDF, ZIP, etc.
    }
}
