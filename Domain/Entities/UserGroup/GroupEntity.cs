using Domain.Entities.Files;
using Domain.Entities.Permission;

namespace Domain.Entities.UserGroup
{
    public class GroupEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double TotalSpace { get; set; }
        public double AvailableSpace { get; set; }
        public DateTime Created { get; set; }

        public ICollection<UserGroupEntity> UserGroups { get; set; } = new List<UserGroupEntity>();
        public ICollection<UserGroupRoleEntity> UserGroupRoles { get; set; } = new List<UserGroupRoleEntity>();
        public ICollection<FileEntity> UploadedFiles { get; set; }
        public ICollection<FileGroupShareEntity> SharedFiles { get; set; }
    }
}
