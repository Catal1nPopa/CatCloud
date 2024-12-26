using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities.Auth;
using Domain.Interfaces;
using Helper.Serilog;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class AuthService(IAuthRepository authRepository, IConfiguration configuration) : IAuthService
    {
        private readonly IAuthRepository _authRepository = authRepository;
        private readonly IConfiguration _configuration = configuration;
        public async Task<string> GetAuthentication(UserCredentialDTO authRequest)
        {
            try
            {
                var user = await _authRepository.GetUserByUsername(authRequest.UserName);
                if (user == null || !user.CheckPassword(authRequest.Password))
                {
                    return null;
                }

                var jwtHandler = new JwtSecurityTokenHandler();
                string getKey = _configuration.GetSection("Jwt").GetSection("SecretKey").Value;
                var key = Encoding.ASCII.GetBytes(getKey);
                var identity = new ClaimsIdentity(new Claim[]
                {
                //new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                });

                var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = identity,
                    Expires = DateTime.Now.AddMinutes(7),
                    SigningCredentials = credentials
                };
                var token = jwtHandler.CreateToken(tokenDescriptor);
                return jwtHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogWarning($"Eroare la autentificare username |{authRequest.UserName}|");
                return ex.Message.ToString();
            }
        }
        string HashPassword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(32);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                350000, //iteration
                HashAlgorithmName.SHA512,
                32);
            return Convert.ToHexString(hash);
        }
        public async Task CreateUser(UserDTO userData)
        {
            var passwordHash = HashPassword(userData.Password, out var salt);
            var user = new UserEntity(userData.Username, passwordHash, Convert.ToHexString(salt), userData.Email, userData.EmailConfirmed,
                userData.TotalStorage, userData.AvailableStorage, DateTime.UtcNow);
            await _authRepository.AddUser(user.Adapt<UserEntity>());
        }
    }
}
