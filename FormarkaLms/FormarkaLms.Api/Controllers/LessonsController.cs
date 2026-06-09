using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FormarkaLms.Application.Lessons.Commands;

namespace FormarkaLms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LessonsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteLesson(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _mediator.Send(new CompleteLessonCommand(id, userId));
        if (!result) return NotFound();

        return Ok(new { Message = "Lesson marked as completed" });
    }
}
