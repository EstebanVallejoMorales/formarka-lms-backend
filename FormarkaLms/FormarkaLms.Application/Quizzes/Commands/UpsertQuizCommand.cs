using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Domain.Entities;
using FormarkaLms.Domain.Enums;
using FormarkaLms.Application.Quizzes.DTOs;

namespace FormarkaLms.Application.Quizzes.Commands;

public record UpsertQuizCommand(QuizAdminDto Quiz) : IRequest<int>;

public class UpsertQuizCommandHandler : IRequestHandler<UpsertQuizCommand, int>
{
    private readonly IApplicationDbContext _context;

    public UpsertQuizCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(UpsertQuizCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Quiz;
        Quiz? quiz = null;

        if (dto.Id.HasValue)
        {
            quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(qs => qs.Options)
                .FirstOrDefaultAsync(q => q.Id == dto.Id.Value, cancellationToken);
        }

        if (quiz == null)
        {
            quiz = new Quiz
            {
                LessonId = dto.LessonId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Quizzes.Add(quiz);
        }

        quiz.Title = dto.Title;
        quiz.Description = dto.Description;
        quiz.PassingScore = dto.PassingScore;
        quiz.UpdatedAt = DateTime.UtcNow;

        // Synchronize Questions
        var existingQuestions = quiz.Questions.ToList();
        var incomingQuestionIds = dto.Questions.Where(q => q.Id.HasValue).Select(q => q.Id!.Value).ToList();

        foreach (var eq in existingQuestions)
        {
            if (!incomingQuestionIds.Contains(eq.Id))
            {
                _context.QuizQuestions.Remove(eq);
            }
        }

        int qOrder = 1;
        foreach (var qDto in dto.Questions)
        {
            var question = qDto.Id.HasValue
                ? existingQuestions.FirstOrDefault(eq => eq.Id == qDto.Id.Value)
                : null;

            if (question != null)
            {
                question.Text = qDto.Text;
                question.QuestionType = Enum.Parse<QuestionType>(qDto.Type, true);
                question.Points = qDto.Points;
                question.Order = qOrder++;
                question.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                question = new QuizQuestion
                {
                    Text = qDto.Text,
                    QuestionType = Enum.Parse<QuestionType>(qDto.Type, true),
                    Points = qDto.Points,
                    Order = qOrder++,
                    CreatedAt = DateTime.UtcNow
                };
                quiz.Questions.Add(question);
            }

            // Synchronize Options
            var existingOptions = question.Options.ToList();
            var incomingOptionIds = qDto.Options.Where(o => o.Id.HasValue).Select(o => o.Id!.Value).ToList();

            foreach (var eo in existingOptions)
            {
                if (!incomingOptionIds.Contains(eo.Id))
                {
                    _context.QuizOptions.Remove(eo);
                }
            }

            foreach (var oDto in qDto.Options)
            {
                var option = oDto.Id.HasValue
                    ? existingOptions.FirstOrDefault(eo => eo.Id == oDto.Id.Value)
                    : null;

                if (option != null)
                {
                    option.Text = oDto.Text;
                    option.IsCorrect = oDto.IsCorrect;
                    option.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    option = new QuizOption
                    {
                        Text = oDto.Text,
                        IsCorrect = oDto.IsCorrect,
                        CreatedAt = DateTime.UtcNow
                    };
                    question.Options.Add(option);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return quiz.Id;
    }
}
