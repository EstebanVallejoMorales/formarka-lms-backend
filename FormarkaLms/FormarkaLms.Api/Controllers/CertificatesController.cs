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

    /// <summary>
    /// Returns the certificate for a completed course, or 400 if the student is not yet eligible.
    /// The error message indicates the specific reason (incomplete lessons, pending/failed deliverables).
    /// </summary>
    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<CertificateDto>> GetCertificate(int courseId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _mediator.Send(new GetCertificateQuery(courseId, userId));
        if (result == null)
            return BadRequest(new { Message = "No puedes obtener el certificado todavía. Asegúrate de haber completado el 100% del curso y que todos tus entregables estén revisados y aprobados." });

        return Ok(result);
    }
}
