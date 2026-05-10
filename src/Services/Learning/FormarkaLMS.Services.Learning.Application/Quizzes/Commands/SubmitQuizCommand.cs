using FormarkaLMS.Services.Learning.Domain.Entities;
using FormarkaLMS.Services.Learning.Domain.Interfaces;
using MediatR;

namespace FormarkaLMS.Services.Learning.Application.Quizzes.Commands;

public record QuizSubmissionDto(Guid QuestionId, Guid SelectedOptionId);
public record SubmitQuizCommand(Guid StudentId, Guid QuizId, List<QuizSubmissionDto> Submissions) : IRequest<QuizResultDto>;
public record QuizResultDto(int Score, bool IsPassed);

public class SubmitQuizHandler : IRequestHandler<SubmitQuizCommand, QuizResultDto>
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizResultRepository _resultRepository;

    public SubmitQuizHandler(IQuizRepository quizRepository, IQuizResultRepository resultRepository)
    {
        _quizRepository = quizRepository;
        _resultRepository = resultRepository;
    }

    public async Task<QuizResultDto> Handle(SubmitQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _quizRepository.GetByIdAsync(request.QuizId);
        if (quiz == null) throw new Exception("Quiz not found");

        int correctAnswers = 0;
        foreach (var submission in request.Submissions)
        {
            var question = quiz.Questions.FirstOrDefault(q => q.Id == submission.QuestionId);
            if (question != null)
            {
                var selectedOption = question.Options.FirstOrDefault(o => o.Id == submission.SelectedOptionId);
                if (selectedOption != null && selectedOption.IsCorrect)
                {
                    correctAnswers++;
                }
            }
        }

        int totalQuestions = quiz.Questions.Count;
        int score = totalQuestions > 0 ? (correctAnswers * 100) / totalQuestions : 0;
        bool isPassed = score >= quiz.PassingScore;

        var result = new QuizResult
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            QuizId = request.QuizId,
            Score = score,
            IsPassed = isPassed,
            TakenAt = DateTime.UtcNow
        };

        await _resultRepository.AddAsync(result);

        return new QuizResultDto(score, isPassed);
    }
}
