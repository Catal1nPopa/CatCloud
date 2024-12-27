using Domain.Entities.Auth;
using Domain.Entities.UserGroup;

namespace Domain.Entities.Permission
{
    public class UserGroupRoleEntity
    {
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        public Guid GroupId { get; set; }
        public GroupEntity Group { get; set; }

        public Guid RoleId { get; set; }
        public RoleEntity Role { get; set; }
    }
}
