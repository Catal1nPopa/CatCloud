using Application.DTOs.Auth;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> GetAuthentication(UserCredentialDTO userCredential);
        Task CreateUser(UserDTO user);
    }
}
