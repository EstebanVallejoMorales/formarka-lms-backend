namespace FormarkaLMS.Shared.Infrastructure.Models
{
    public class Enrollment
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public bool IsCompleted { get; set; }
        public double ProgressPercentage { get; set; }
    }
}
