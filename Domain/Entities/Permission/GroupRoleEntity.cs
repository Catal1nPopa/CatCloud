using Domain.Entities.UserGroup;

namespace Domain.Entities.Permission
{
    public class GroupRoleEntity
    {
        public Guid GroupId { get; set; }
        public GroupEntity Group { get; set; }

        public Guid RoleId { get; set; }
        public RoleEntity Role { get; set; }
    }
}
