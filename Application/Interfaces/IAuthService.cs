using Application.DTOs.Auth;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> GetAuthentication(UserCredentialDTO userCredential);
        Task CreateUser(UserDTO user);
        Task DeleteUser(Guid userId);
        Task<List<UserInfoDTO>> GetUsers();
        Task<List<UserInfoDTO>> GetUsersToShareFile(Guid fileId);
        Task UpdateUser(UserInfoDTO user);
        Task<UserInfoDTO> GetUser();
        Task<UserInfoDTO> GetUserById(Guid userId);
    }
}
