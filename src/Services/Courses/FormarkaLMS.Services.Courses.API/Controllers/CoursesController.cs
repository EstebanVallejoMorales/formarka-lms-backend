using FormarkaLMS.Services.Courses.Application.Courses.Commands;
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

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateCourse(CreateCourseDto courseDto)
    {
        var id = await _mediator.Send(new CreateCourseCommand(courseDto));
        return Ok(id);
    }
}
