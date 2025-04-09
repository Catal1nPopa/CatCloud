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
    public class AuthService(IAuthRepository authRepository, 
        IConfiguration configuration,
        IUserProvider userProvider) : IAuthService
    {
        private readonly IAuthRepository _authRepository = authRepository;
        private readonly IConfiguration _configuration = configuration;
        private readonly IUserProvider _userProvider = userProvider;
        public async Task<string> GetAuthentication(UserCredentialDTO authRequest)
        {
            try
            {
                var user = await _authRepository.GetUserByUsername(authRequest.UserName);
                if (user == null || !user.CheckPassword(authRequest.Password))
                {
                    return null;
                }
                if (!user.Enabled)
                {
                    throw new Exception("Contul respectiv nu este activat, contactati suportul");
                }

                var jwtHandler = new JwtSecurityTokenHandler();
                string getKey = _configuration.GetSection("Jwt").GetSection("Key").Value;
                var key = Encoding.UTF8.GetBytes(getKey);
                var identity = new ClaimsIdentity(new Claim[]
                {
                //new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                });

                var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = identity,
                    Expires = DateTime.Now.AddMinutes(60),
                    SigningCredentials = credentials,
                    Issuer = _configuration.GetSection("Jwt").GetSection("Issuer").Value,
                    Audience = _configuration.GetSection("Jwt").GetSection("Audience").Value
                };
                var token = jwtHandler.CreateToken(tokenDescriptor);
                return jwtHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogWarning($"Eroare la autentificare username |{authRequest.UserName}|");
                throw new Exception(ex.Message);
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
            long totalStorage = (long)(userData.TotalStorage * 1073741824);
            userData.AvailableStorage = totalStorage;
            userData.TotalStorage = totalStorage;
            var user = new UserEntity(userData.Username, passwordHash, Convert.ToHexString(salt), userData.Email, userData.EmailConfirmed,
                userData.TotalStorage, userData.AvailableStorage, DateTime.UtcNow);
            await _authRepository.AddUser(user.Adapt<UserEntity>());
        }

        public async Task DeleteUser(Guid userId)
        {
            await _authRepository.DeleteUser(userId);
        }

        public async Task<List<UserInfoDTO>> GetUsers()
        {
            var users = await _authRepository.GetAllUsers();
            return users.Adapt<List<UserInfoDTO>>();
        }

        public async Task<List<UserInfoDTO>> GetUsersToShareFile(Guid fileId)
        {
            Guid userId = _userProvider.GetUserId();
            List<UserEntity> users = await _authRepository.GetUsersNotSharedWithFile(fileId, userId);
            return users.Adapt<List<UserInfoDTO>>();
        }

        public async Task<UserInfoDTO> GetUser()
        {
            Guid userId = _userProvider.GetUserId();
            var user = await _authRepository.GetUserById(userId);
            return user.Adapt<UserInfoDTO>();
        }

        public async Task UpdateUser(UserInfoDTO user)
        {
            await _authRepository.UpdateUser(user.Adapt<UserInfoEntity>());
        }
    }
}
