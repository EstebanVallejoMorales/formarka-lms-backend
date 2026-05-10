using FormarkaLMS.Services.Learning.Application.Enrollments.Commands;
using FormarkaLMS.Services.Learning.Application.Progress.Commands;
using FormarkaLMS.Services.Learning.Application.Quizzes.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FormarkaLMS.Gateway.Controllers;

[ApiController]
[Route("api/learning")]
public class GatewayLearningController : ControllerBase
{
    private readonly IMediator _mediator;

    public GatewayLearningController(IMediator mediator)
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

    // Queries for enrollments and progress would be added here.
    // Example:
    // [HttpGet("enrollments/{studentId}")]
    // public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetStudentEnrollments(Guid studentId)
    // {
    //     var query = new GetStudentEnrollmentsQuery(studentId);
    //     return Ok(await _mediator.Send(query));
    // }
}
