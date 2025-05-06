using Domain.Entities.Auth;

namespace Domain.Interfaces
{
    public interface IAuthRepository
    {
        Task AddUser(UserEntity userEntity);
        Task<UserEntity> GetUserByUsername(string username);
        Task<UserEntity> GetUserById(Guid userId);
        Task<UserEntity> GetUserByEmail(string email);
        Task DeleteUser(Guid userId);
        Task UpdateUser(UserInfoEntity userEntity);
        Task<List<UserInfoEntity>> GetAllUsers();
        Task<bool> DecreaseAvailableSize(long fileSize, Guid userId);
        Task<bool> IncreaseAvailableSize(long fileSize, Guid userId);
        Task<List<UserEntity>> GetUsersNotSharedWithFile(Guid fileId, Guid currentUserId);
        Task ConfirmEmail(string token);
        Task ResetPassword(string userId, string newPassword, string newPasswordSalt);
    }
}
