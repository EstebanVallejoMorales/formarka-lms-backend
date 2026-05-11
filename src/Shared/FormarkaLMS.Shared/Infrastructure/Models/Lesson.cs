namespace FormarkaLMS.Shared.Infrastructure.Models
{
    public class Lesson
    {
        public Guid Id { get; set; }
        public Guid ModuleId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty; // Rich text or description
        public string VideoUrl { get; set; } = string.Empty;
        public string ExternalUrl { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsPreview { get; set; }

        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
