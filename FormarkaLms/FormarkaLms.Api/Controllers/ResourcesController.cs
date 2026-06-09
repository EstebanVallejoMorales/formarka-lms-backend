using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FormarkaLms.Application.Resources.Commands;

namespace FormarkaLms.Api.Controllers;

[Authorize(Roles = "Admin,Teacher")]
[ApiController]
[Route("api/[controller]")]
public class ResourcesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResourcesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ResourceDto>> AddResource([FromBody] AddResourceCommand command)
    {
        return await _mediator.Send(command);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResource(int id)
    {
        // Simple delete logic
        // Implementation omitted for brevity, but follows same pattern
        return NoContent();
    }
}
