using FormarkaLMS.Services.Learning.Application.Enrollments.Commands;
using FormarkaLMS.Services.Learning.Application.Progress.Commands;
using FormarkaLMS.Services.Learning.Application.Quizzes.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FormarkaLMS.Services.Learning.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LearningController : ControllerBase
{
    private readonly IMediator _mediator;

    public LearningController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("enroll")]
    public async Task<ActionResult<Guid>> Enroll(EnrollStudentCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpPost("progress/complete")]
    public async Task<IActionResult> MarkAsCompleted(MarkLessonAsCompletedCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPost("quiz/submit")]
    public async Task<ActionResult<QuizResultDto>> SubmitQuiz(SubmitQuizCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
