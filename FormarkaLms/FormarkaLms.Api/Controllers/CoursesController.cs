using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FormarkaLms.Application.Courses.Queries;

using FormarkaLms.Application.Courses.Commands;

namespace FormarkaLms.Api.Controllers;

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
    public async Task<ActionResult<List<CourseDto>>> GetCourses()
    {
        return await _mediator.Send(new GetCoursesQuery());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDetailDto>> GetCourse(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var course = await _mediator.Send(new GetCourseByIdQuery(id, userId));
        if (course == null) return NotFound();
        return Ok(course);
    }

    [Authorize]
    [HttpPost("{id}/enroll")]
    public async Task<IActionResult> Enroll(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _mediator.Send(new EnrollInCourseCommand(id, userId));
        if (!result) return NotFound();

        return Ok(new { Message = "Successfully enrolled in course" });
    }

    [Authorize]
    [HttpGet("my-courses")]
    public async Task<ActionResult<List<CourseDto>>> GetMyCourses()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        return await _mediator.Send(new GetEnrolledCoursesQuery(userId));
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpGet("{id}/students")]
    public async Task<ActionResult<List<StudentProgressDto>>> GetEnrolledStudents(int id)
    {
        return await _mediator.Send(new GetEnrolledStudentsQuery(id));
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateCourseCommand command)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        command.InstructorId = userId!; // Default to current user as instructor
        return await _mediator.Send(command);
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCourseCommand command)
    {
        if (id != command.Id) return BadRequest();
        var result = await _mediator.Send(command);
        if (!result) return NotFound();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/assign-instructor")]
    public async Task<IActionResult> AssignInstructor(int id, [FromBody] string instructorId)
    {
        var result = await _mediator.Send(new AssignInstructorCommand(id, instructorId));
        if (!result) return BadRequest(new { Message = "Instructor not found or invalid role." });
        return Ok(new { Message = "Instructor assigned successfully" });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteCourseCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }
}

