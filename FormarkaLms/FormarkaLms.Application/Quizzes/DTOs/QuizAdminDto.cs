namespace FormarkaLms.Application.Quizzes.DTOs;

public class QuizAdminDto
{
    public int? Id { get; set; }
    public int LessonId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public decimal PassingScore { get; set; }
    public List<QuestionAdminDto> Questions { get; set; } = new();
}

public class QuestionAdminDto
{
    public int? Id { get; set; }
    public string Text { get; set; } = default!;
    public string Type { get; set; } = "multiplechoice";
    public int Points { get; set; }
    public List<OptionAdminDto> Options { get; set; } = new();
}

public class OptionAdminDto
{
    public int? Id { get; set; }
    public string Text { get; set; } = default!;
    public bool IsCorrect { get; set; }
}
