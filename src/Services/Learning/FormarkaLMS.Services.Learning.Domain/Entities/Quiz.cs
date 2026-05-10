namespace FormarkaLMS.Services.Learning.Domain.Entities;

public class Quiz
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int PassingScore { get; set; }
    
    public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
}

public class QuizQuestion
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    
    public ICollection<QuizOption> Options { get; set; } = new List<QuizOption>();
}

public class QuizOption
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

public class QuizResult
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid QuizId { get; set; }
    public int Score { get; set; }
    public bool IsPassed { get; set; }
    public DateTime TakenAt { get; set; }
}
