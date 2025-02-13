namespace Domain.Entities.Permission
{
    public class GroupPermissionsEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<UserGroupPermissionsEntity> UserGroupPermissions { get; set; } = new List<UserGroupPermissionsEntity>();

    }
}
