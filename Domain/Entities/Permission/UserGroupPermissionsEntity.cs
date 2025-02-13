using Domain.Entities.Auth;
using Domain.Entities.UserGroup;

namespace Domain.Entities.Permission
{
    public class UserGroupPermissionsEntity
    {
        public Guid GroupId { get; set; }
        public GroupEntity Group { get; set; }

        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        public Guid PermissionId { get; set; }
        public GroupPermissionsEntity Permission { get; set; }
    }
}
