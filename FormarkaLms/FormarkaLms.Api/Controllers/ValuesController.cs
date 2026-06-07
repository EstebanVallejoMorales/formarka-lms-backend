using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FormarkaLms.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult GetPublic() => Ok("Todos pueden ver esto");

        [Authorize] // Protected by Supabase's JWT
        [HttpGet("private")]
        public IActionResult GetPrivate()
        {
            // Supabase stores the user ID in the 'sub' claim (NameIdentifier)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);

            return Ok(new
            {
                Mensaje = "¡Lograste entrar al backend protegido!",
                UserId = userId,
                Email = email
            });
        }
    }
}
