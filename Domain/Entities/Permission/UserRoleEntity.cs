using Domain.Entities.Auth;

namespace Domain.Entities.Permission
{
    public class UserRoleEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        public Guid RoleId { get; set; }
        public RoleEntity Role { get; set; }
    }
}
