using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;

namespace FormarkaLms.Application.Quizzes.Queries;

public class GetQuizByLessonIdQueryHandler : IRequestHandler<GetQuizByLessonIdQuery, QuizDto?>
{
    private readonly IApplicationDbContext _context;

    public GetQuizByLessonIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<QuizDto?> Handle(GetQuizByLessonIdQuery request, CancellationToken cancellationToken)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Questions.OrderBy(qs => qs.Order))
            .ThenInclude(qs => qs.Options)
            .FirstOrDefaultAsync(q => q.LessonId == request.LessonId, cancellationToken);

        if (quiz == null) return null;

        return new QuizDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            PassingScore = quiz.PassingScore,
            Questions = quiz.Questions.Select(qs => new QuizQuestionDto
            {
                Id = qs.Id,
                Text = qs.Text,
                Type = qs.QuestionType.ToString().ToLower(),
                Points = qs.Points,
                Options = qs.Options.Select(o => new QuizOptionDto
                {
                    Id = o.Id,
                    Text = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToList()
            }).ToList()
        };
    }
}
