using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FormarkaLms.Application.Comments.Commands;
using FormarkaLms.Application.Comments.Queries;
using FormarkaLms.Application.Comments.DTOs;

namespace FormarkaLms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("lesson/{lessonId}")]
    public async Task<ActionResult<List<CommentDto>>> GetComments(int lessonId)
    {
        return await _mediator.Send(new GetCommentsQuery(lessonId));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CommentDto>> AddComment([FromBody] AddCommentRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new AddCommentCommand(request.LessonId, userId, request.Content, request.ParentId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

public class AddCommentRequest
{
    public int LessonId { get; set; }
    public string Content { get; set; } = default!;
    public int? ParentId { get; set; }
}
