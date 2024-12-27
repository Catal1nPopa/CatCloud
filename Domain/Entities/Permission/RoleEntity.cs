namespace Domain.Entities.Permission
{
    public class RoleEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<RolePermissionEntity> RolePermissions { get; set; } = new List<RolePermissionEntity>();
        public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
    }
}
