using Domain.Entities.Auth;
using Domain.Entities.UserGroup;

namespace Domain.Entities.Permission
{
    public class RoleEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<PermissionEntity> Permissions { get; set; } = new List<PermissionEntity>();
        public ICollection<RolePermissionEntity> RolePermissions { get; set; } = new List<RolePermissionEntity>();

        public ICollection<UserEntity> Users { get; set; }
        public ICollection<GroupEntity> Groups { get; set; }
    }
}
