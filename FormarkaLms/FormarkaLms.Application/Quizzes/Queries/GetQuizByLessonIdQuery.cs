using MediatR;

namespace FormarkaLms.Application.Quizzes.Queries;

public class QuizDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public decimal PassingScore { get; set; }
    public List<QuizQuestionDto> Questions { get; set; } = new();
}

public class QuizQuestionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = default!;
    public string Type { get; set; } = "multiplechoice";
    public int Points { get; set; }
    public List<QuizOptionDto> Options { get; set; } = new();
}

public class QuizOptionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = default!;
    public bool IsCorrect { get; set; }
}

public record GetQuizByLessonIdQuery(int LessonId) : IRequest<QuizDto?>;
