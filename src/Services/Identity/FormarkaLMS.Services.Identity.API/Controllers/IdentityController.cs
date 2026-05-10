using FormarkaLMS.Services.Identity.Application.DTOs;
using FormarkaLMS.Services.Identity.Application.Users.Commands;
using FormarkaLMS.Services.Identity.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FormarkaLMS.Services.Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    private readonly IMediator _mediator;

    public IdentityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserProfileDto>> GetProfile(Guid id)
    {
        var query = new GetUserProfileByIdQuery(id);
        var result = await _mediator.Send(query);
        
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfile(Guid id, UpdateUserProfileDto updateDto)
    {
        var command = new UpdateUserProfileCommand(id, updateDto);
        var result = await _mediator.Send(command);
        
        if (!result) return NotFound();
        return NoContent();
    }
}
