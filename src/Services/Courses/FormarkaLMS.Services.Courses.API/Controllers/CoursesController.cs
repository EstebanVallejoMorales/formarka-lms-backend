using FormarkaLMS.Services.Courses.Application.Courses.Commands;
using FormarkaLMS.Services.Courses.Application.Courses.Queries;
using FormarkaLMS.Services.Courses.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FormarkaLMS.Services.Courses.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetAllCourses()
    {
        return Ok(await _mediator.Send(new GetAllCoursesQuery()));
    }

    [HttpGet("published")]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetPublishedCourses()
    {
        return Ok(await _mediator.Send(new GetPublishedCoursesQuery()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetCourseById(Guid id)
    {
        var course = await _mediator.Send(new GetCourseByIdQuery(id));
        if (course == null) return NotFound();
        return Ok(course);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateCourse(CreateCourseDto courseDto)
    {
        var id = await _mediator.Send(new CreateCourseCommand(courseDto));
        return Ok(id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(Guid id, CreateCourseDto courseDto)
    {
        var result = await _mediator.Send(new UpdateCourseCommand(id, courseDto));
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        var result = await _mediator.Send(new DeleteCourseCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPatch("{id}/publish")]
    public async Task<IActionResult> PublishCourse(Guid id)
    {
        var result = await _mediator.Send(new PublishCourseCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPatch("{id}/unpublish")]
    public async Task<IActionResult> UnpublishCourse(Guid id)
    {
        var result = await _mediator.Send(new UnpublishCourseCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }
}
