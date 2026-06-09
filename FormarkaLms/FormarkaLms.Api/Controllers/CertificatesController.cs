using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FormarkaLms.Application.Certificates.Queries;
using FormarkaLms.Application.Certificates.DTOs;

namespace FormarkaLms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CertificatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CertificatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<CertificateDto>> GetCertificate(int courseId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _mediator.Send(new GetCertificateQuery(courseId, userId));
        if (result == null) return BadRequest(new { Message = "Course not completed yet." });

        return Ok(result);
    }
}
