using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Domain.Entities;

namespace FormarkaLms.Application.Quizzes.Commands;

public record SubmitQuizAttemptCommand(int QuizId, string UserId, Dictionary<int, int> Answers) : IRequest<QuizResultDto>;

public class QuizResultDto
{
    public decimal Score { get; set; }
    public bool Passed { get; set; }
}

public class SubmitQuizAttemptCommandHandler : IRequestHandler<SubmitQuizAttemptCommand, QuizResultDto>
{
    private readonly IApplicationDbContext _context;

    public SubmitQuizAttemptCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<QuizResultDto> Handle(SubmitQuizAttemptCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(qs => qs.Options)
            .FirstOrDefaultAsync(q => q.Id == request.QuizId, cancellationToken);

        if (quiz == null) throw new Exception("Quiz not found");

        decimal totalPoints = quiz.Questions.Sum(q => q.Points);
        decimal earnedPoints = 0;

        foreach (var question in quiz.Questions)
        {
            if (request.Answers.TryGetValue(question.Id, out int selectedOptionId))
            {
                var option = question.Options.FirstOrDefault(o => o.Id == selectedOptionId);
                if (option != null && option.IsCorrect)
                {
                    earnedPoints += question.Points;
                }
            }
        }

        decimal score = totalPoints > 0 ? (earnedPoints / totalPoints) * 100 : 0;
        bool passed = score >= quiz.PassingScore;

        var attempt = new QuizAttempt
        {
            QuizId = request.QuizId,
            StudentId = request.UserId,
            FinalScore = score,
            SubmittedAt = DateTime.UtcNow
        };

        _context.QuizAttempts.Add(attempt);
        await _context.SaveChangesAsync(cancellationToken);

        return new QuizResultDto
        {
            Score = score,
            Passed = passed
        };
    }
}
