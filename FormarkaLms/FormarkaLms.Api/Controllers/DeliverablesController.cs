using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FormarkaLms.Application.Deliverables.Common;

namespace FormarkaLms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DeliverablesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeliverablesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("lesson/{lessonId}")]
    public async Task<ActionResult<DeliverableDto>> GetMyDeliverable(int lessonId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _mediator.Send(new GetDeliverableByLessonQuery(lessonId, userId));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost("lesson/{lessonId}")]
    public async Task<ActionResult<DeliverableDto>> Submit(int lessonId, [FromBody] string contentUrl)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _mediator.Send(new SubmitDeliverableCommand(lessonId, userId, contentUrl));
        return Ok(result);
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpGet("course/{courseId}/student/{studentId}")]
    public async Task<ActionResult<List<DeliverableDto>>> GetStudentDeliverables(int courseId, string studentId)
    {
        return await _mediator.Send(new GetStudentDeliverablesInCourseQuery(courseId, studentId));
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpPost("{id}/grade")]
    public async Task<IActionResult> Grade(int id, [FromBody] GradeRequest request)
    {
        var result = await _mediator.Send(new GradeDeliverableCommand(id, request.Grade, request.Feedback));
        if (!result) return NotFound();
        return Ok(new { Message = "Deliverable graded successfully" });
    }

    public class GradeRequest
    {
        public decimal Grade { get; set; }
        public string? Feedback { get; set; }
    }
}
