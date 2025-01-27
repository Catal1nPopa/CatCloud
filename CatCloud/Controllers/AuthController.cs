using Application.DTOs.Auth;
using Application.Interfaces;
using CatCloud.Models.User;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace CatCloud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserCredentialsModel userCredentials)
        {
            var token = await _authService.GetAuthentication(userCredentials.Adapt<UserCredentialDTO>());
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Autentificare eșuată. Verifică credentialele." });
            }

            return Ok(new { token = token });
        }

        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUser(UserModel userData)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _authService.CreateUser(userData.Adapt<UserDTO>());
            return Created();
        }
    }
}
