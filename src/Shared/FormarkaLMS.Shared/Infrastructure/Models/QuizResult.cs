namespace FormarkaLMS.Shared.Infrastructure.Models
{
    public class QuizResult
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid QuizId { get; set; }
        public int Score { get; set; }
        public bool IsPassed { get; set; }
        public DateTime TakenAt { get; set; }
    }
}
