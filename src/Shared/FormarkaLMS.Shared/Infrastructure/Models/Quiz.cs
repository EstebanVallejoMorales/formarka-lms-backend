namespace FormarkaLMS.Shared.Infrastructure.Models
{
    public class Quiz
    {
        public Guid Id { get; set; }
        public Guid LessonId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int PassingScore { get; set; }

        public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
    }
}
