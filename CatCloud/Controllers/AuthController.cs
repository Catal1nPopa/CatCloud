using Application.DTOs.Auth;
using Application.Interfaces;
using CatCloud.Models.User;
using Mapster;
using Microsoft.AspNetCore.Authorization;
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
            try {
                var token = await _authService.GetAuthentication(userCredentials.Adapt<UserCredentialDTO>());
                if (token != null) 
                {
                    return Ok(new { token = token }); 
                }
                return BadRequest("Credentiale invalide");
            }
            catch(Exception ex) {
                throw new Exception(ex.Message);
                //return BadRequest(new { message = "Eroare la autentificare ${" });
            }
        }

        //[Authorize]
        [HttpPost("User")]
        public async Task<IActionResult> CreateUser(UserModel userData)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                await _authService.CreateUser(userData.Adapt<UserDTO>());
                return Ok("Utilizator creat");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[Authorize]
        [HttpDelete("User")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            await _authService.DeleteUser(userId);
            return Ok();
        }

        [Authorize]
        [HttpGet("User")]
        public async Task<ActionResult<List<UserInfoModel>>> GetAllUsers()
        {
            try
            {
                var users = await _authService.GetUsers();
                return Ok(users.Adapt<List<UserInfoModel>>());
            }
            catch(Exception ex) { return BadRequest(ex.Message); }
        }

        [Authorize]
        [HttpGet("usersToShare")]
        public async Task<ActionResult<List<UserInfoModel>>> GetUsersToShareFile(Guid fileId)
        {
            try
            {
                var users = await _authService.GetUsersToShareFile(fileId);
                return Ok(users.Adapt<List<UserInfoModel>>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
