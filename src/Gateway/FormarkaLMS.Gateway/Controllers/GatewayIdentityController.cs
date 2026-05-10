using FormarkaLMS.Services.Identity.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FormarkaLMS.Gateway.Controllers;

[ApiController]
[Route("api/identity")]
public class GatewayIdentityController : ControllerBase
{
    private readonly IMediator _mediator;

    public GatewayIdentityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("profile/{id}")] // Example: To get a user profile
    public async Task<ActionResult> GetUserProfile(Guid id)
    {
        // Note: This assumes you have a query to get a profile by ID in the Identity service.
        // You might need to adapt the existing query or create a new one if it's only for the logged-in user.
        var query = new GetUserProfileByIdQuery(id); // Assuming this query exists and is registered with MediatR
        var profile = await _mediator.Send(query);
        if (profile == null) return NotFound();
        return Ok(profile);
    }

    // Other Identity-related endpoints (Login, Register, etc.) would typically be handled directly by the Identity service's API
    // and might be proxied or called here if needed for gateway-specific orchestration.
}
