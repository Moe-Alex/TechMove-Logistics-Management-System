using Microsoft.AspNetCore.Mvc;
using TechMove.Api.Services;

namespace TechMove.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly IFirebaseAuthService _firebaseAuthService;

        public AuthApiController(IFirebaseAuthService firebaseAuthService)
        {
            _firebaseAuthService = firebaseAuthService;
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyToken([FromBody] VerifyFirebaseTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.IdToken))
            {
                return BadRequest(new { message = "Firebase ID token is required." });
            }

            var uid = await _firebaseAuthService.VerifyTokenAsync(request.IdToken);

            if (uid == null)
            {
                return Unauthorized(new { message = "Invalid Firebase token." });
            }

            return Ok(new
            {
                message = "Firebase token is valid.",
                uid
            });
        }
    }

    public class VerifyFirebaseTokenRequest
    {
        public string IdToken { get; set; } = string.Empty;
    }
}