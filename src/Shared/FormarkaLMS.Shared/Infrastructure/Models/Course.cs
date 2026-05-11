using FormarkaLMS.Shared.Infrastructure.Enums;
using System.Reflection;

namespace FormarkaLMS.Shared.Infrastructure.Models
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public CourseLevel Level { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Module> Modules { get; set; } = new List<Module>();
    }
}
