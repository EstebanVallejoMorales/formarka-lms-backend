using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FormarkaLms.Application.Quizzes.Queries;
using FormarkaLms.Application.Quizzes.Commands;

using FormarkaLms.Application.Quizzes.DTOs;

namespace FormarkaLms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class QuizzesController : ControllerBase
{
    private readonly IMediator _mediator;

    public QuizzesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("lesson/{lessonId}")]
    public async Task<ActionResult<QuizDto>> GetQuizByLesson(int lessonId)
    {
        var quiz = await _mediator.Send(new GetQuizByLessonIdQuery(lessonId));
        if (quiz == null) return NotFound();
        return Ok(quiz);
    }

    [HttpPost("{id}/submit")]
    public async Task<ActionResult<QuizResultDto>> SubmitAttempt(int id, [FromBody] Dictionary<int, int> answers)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _mediator.Send(new SubmitQuizAttemptCommand(id, userId, answers));
        return Ok(result);
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpPost("upsert")]
    public async Task<ActionResult<int>> Upsert([FromBody] QuizAdminDto quiz)
    {
        return await _mediator.Send(new UpsertQuizCommand(quiz));
    }
}

