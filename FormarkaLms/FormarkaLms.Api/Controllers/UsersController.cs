using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FormarkaLms.Application.Users.Commands;
using FormarkaLms.Application.Users.Queries;
using FormarkaLms.Application.Users.DTOs;

namespace FormarkaLms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("profile/complete")]
    public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileDto dto)
    {
        // 1. Extraemos el UID y el Email directamente del Token JWT de Supabase
        var supabaseId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email); 

        if (string.IsNullOrEmpty(supabaseId))
            return Unauthorized(new { Message = "Token inválido o sin ID de usuario." });

        // 2. Despachamos el comando hacia la capa de aplicación con MediatR
        var command = new CompleteUserProfileCommand(
            supabaseId, 
            email ?? string.Empty, 
            dto.Name, 
            dto.Role, 
            dto.Specialty, 
            dto.PhotoUrl
        );
        
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(new { Message = result.Errors.FirstOrDefault() });
        }

        return Ok(new { Message = "Perfil completado exitosamente", UserId = result.Data });
    }

    [HttpGet("profile/status")]
    public async Task<IActionResult> GetProfileStatus()
    {
        var supabaseId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(supabaseId))
            return Unauthorized();

        var userExists = await _mediator.Send(new CheckUserExistsQuery(supabaseId));
        
        return Ok(new { IsProfileComplete = userExists });
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpGet("instructors")]
    public async Task<ActionResult<List<InstructorDto>>> GetInstructors()
    {
        return await _mediator.Send(new GetInstructorsQuery());
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<List<UserAdminDto>>> GetAll()
    {
        return await _mediator.Send(new GetAllUsersQuery());
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id) return BadRequest();
        var result = await _mediator.Send(command);
        if (!result) return NotFound();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] string role)
    {
        var result = await _mediator.Send(new UpdateUserRoleCommand(id, role));
        if (!result) return NotFound();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _mediator.Send(new DeleteUserCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }
}
