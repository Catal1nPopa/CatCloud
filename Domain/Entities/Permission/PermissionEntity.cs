namespace Domain.Entities.Permission
{
    public class PermissionEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<RoleEntity> Roles { get; set; }
        public ICollection<RolePermissionEntity> RolePermissions { get; set; } = new List<RolePermissionEntity>();

    }
}
