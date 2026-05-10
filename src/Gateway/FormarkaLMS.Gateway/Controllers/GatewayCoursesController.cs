using FormarkaLMS.Services.Courses.Application.DTOs;
using FormarkaLMS.Services.Courses.Application.Courses.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FormarkaLMS.Gateway.Controllers;

[ApiController]
[Route("api/courses")]
public class GatewayCoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public GatewayCoursesController(IMediator mediator)
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

    // Other Course commands (Create, Update, Delete, Publish, Unpublish) will be handled by the respective microservice APIs
    // or could be proxied here if needed, but for now, we'll assume they are directly accessible if needed.
}
