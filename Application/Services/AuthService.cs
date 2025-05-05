using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities.Auth;
using Domain.Interfaces;
using Helper.Serilog;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
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
                if (!user.EmailConfirmed)
                {
                    throw new Exception("Adresa de email nu a fost confirmată. Te rugăm să verifici emailul.");
                }

                var jwtHandler = new JwtSecurityTokenHandler();
                string getKey = _configuration.GetSection("Jwt").GetSection("Key").Value;
               
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email),    
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                    };
                foreach (var userRole in user.UserRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                }
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddMinutes(60),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(getKey)), SecurityAlgorithms.HmacSha256Signature),
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
            userData.EmailConfirmed = false;
            var confirmEmailToken = Guid.NewGuid() + passwordHash;
            userData.EmailConfirmationToken = confirmEmailToken;
            userData.EmailConfirmationTokenExpires = DateTime.UtcNow.AddDays(1);
            var user = new UserEntity(userData.Username, passwordHash, Convert.ToHexString(salt), userData.Email, userData.EmailConfirmed,
                userData.TotalStorage, userData.AvailableStorage, DateTime.UtcNow, userData.EmailConfirmationToken, userData.EmailConfirmationTokenExpires);
            
            var confirmationLink = $"https://localhost:9001/api/auth/confirm-email?token={confirmEmailToken}";
            var body = $@"
    <div style='font-family: Arial, sans-serif; text-align: center; padding: 20px; background-color: #f9f9f9;'>
        <h2 style='color: #333;'>Salut {user.Username},</h2>
        <p style='font-size: 16px; color: #555;'>Mulțumim că te-ai înregistrat la <strong>Cat Storage</strong>.</p>
        <p style='font-size: 16px; color: #555;'>Pentru a finaliza înregistrarea, te rugăm să confirmi adresa ta de email apăsând pe butonul de mai jos:</p>
        <a href='{confirmationLink}' 
           style='display: inline-block; padding: 12px 24px; margin-top: 20px; background-color: #4caf50; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;'>
            Confirmă Emailul
        </a>
        <p style='margin-top: 40px; font-size: 12px; color: #aaa;'>Dacă nu ai cerut acest email, poți să-l ignori în siguranță.</p>
    </div>";
            await SendEmail(user.Email, "Confirmare email", body);

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

        public async Task<UserInfoDTO> GetUserById(Guid userId)
        {
            var user = await _authRepository.GetUserById(userId);
            return user.Adapt<UserInfoDTO>();
        }

        public async Task ConfirmEmail(string token)
        {
            await _authRepository.ConfirmEmail(token);
        }
        
        private async Task SendEmail(string emailToReceive, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("misterco2002@gmail.com", "yvqs lfrj osaa ygqo");

                    using (var message = new MailMessage(
                               from: new MailAddress("misterco2002@gmail.com","CatStorage"),
                               to: new MailAddress(emailToReceive)))
                    {
                        message.Subject = $"Cat Storage | {subject}!";
                        message.Body = body;
                        message.IsBodyHtml = true;

                        await client.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }
    }
}
