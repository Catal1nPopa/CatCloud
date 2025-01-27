using Domain.Entities.Auth;

namespace Domain.Interfaces
{
    public interface IAuthRepository
    {
        Task AddUser(UserEntity userEntity);
        Task<UserEntity> GetUserByUsername(string username);
        Task<UserEntity> GetUserById(Guid userId);
    }
}
